using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface IDevolucionRepository
    {
        Task<List<DevolucionDto>> ObtenerTodos();
        Task<DevolucionDto?> ObtenerPorId(int id);
        Task<DevolucionDto?> ObtenerPorVentaId(int ventaId);
        Task<DevolucionDto> Crear(DevolucionCreateDto dto);
        Task<DevolucionDto> Gestionar(int id, DevolucionGestionarDto dto);
        Task<List<EstadoDevolucionDto>> ObtenerEstados();
    }

    public class DevolucionRepository : IDevolucionRepository
    {
        private readonly SyspharmaContext _context;
        public DevolucionRepository(SyspharmaContext context) => _context = context;

        private static DevolucionDto MapDto(Devolucion d) => new DevolucionDto
        {
            Id = d.Id,
            VentaId = d.VentaId,
            NumeroVenta = d.Venta?.NumeroVenta,
            UsuarioId = d.UsuarioId,
            UsuarioNombre = d.Usuario?.Nombre,
            EstadoId = d.EstadoId,
            EstadoNombre = d.Estado?.Nombre,
            Motivo = d.Motivo,
            Observaciones = d.Observaciones,
            TotalDevolucion = d.TotalDevolucion,
            FechaDevolucion = d.FechaDevolucion,
            FechaGestion = d.FechaGestion,
            UsuarioGestionId = d.UsuarioGestionId,
            Detalles = d.Detalles.Select(det => new DetalleDevolucionDto
            {
                Id = det.Id,
                DetalleVentaId = det.DetalleVentaId,
                ProductoId = det.ProductoId,
                ProductoNombre = det.Producto?.Nombre,
                CantidadDevuelta = det.CantidadDevuelta,
                PrecioUnitario = det.PrecioUnitario,
                SubtotalDevuelto = det.SubtotalDevuelto ?? 0
            }).ToList()
        };

        private IQueryable<Devolucion> QueryConIncludes() =>
            _context.Devoluciones
                .Include(d => d.Venta)
                .Include(d => d.Usuario)
                .Include(d => d.Estado)
                .Include(d => d.Detalles).ThenInclude(det => det.Producto);

        public async Task<List<DevolucionDto>> ObtenerTodos() =>
            (await QueryConIncludes().OrderByDescending(d => d.FechaDevolucion).ToListAsync())
            .Select(MapDto).ToList();

        public async Task<DevolucionDto?> ObtenerPorId(int id)
        {
            var d = await QueryConIncludes().FirstOrDefaultAsync(d => d.Id == id);
            return d == null ? null : MapDto(d);
        }

        public async Task<DevolucionDto?> ObtenerPorVentaId(int ventaId)
        {
            var d = await QueryConIncludes().FirstOrDefaultAsync(d => d.VentaId == ventaId);
            return d == null ? null : MapDto(d);
        }

        public async Task<DevolucionDto> Crear(DevolucionCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obtener precios de los detalles de venta
                var detalleVentaIds = dto.Detalles.Select(d => d.DetalleVentaId).ToList();
                var detallesVenta = await _context.VentaDetalles
                    .Where(d => detalleVentaIds.Contains(d.Id))
                    .ToListAsync();

                var detalles = dto.Detalles.Select(d =>
                {
                    var dventa = detallesVenta.FirstOrDefault(dv => dv.Id == d.DetalleVentaId);
                    var precio = dventa?.PrecioUnitario ?? 0;
                    return new DetalleDevolucion
                    {
                        DetalleVentaId = d.DetalleVentaId,
                        ProductoId = d.ProductoId,
                        CantidadDevuelta = d.CantidadDevuelta,
                        PrecioUnitario = precio,
                        SubtotalDevuelto = d.CantidadDevuelta * precio
                    };
                }).ToList();

                var total = detalles.Sum(d => d.SubtotalDevuelto ?? 0);

                var devolucion = new Devolucion
                {
                    VentaId = dto.VentaId,
                    UsuarioId = dto.UsuarioId,
                    EstadoId = 1, // pendiente
                    Motivo = dto.Motivo,
                    Observaciones = dto.Observaciones,
                    TotalDevolucion = total,
                    FechaDevolucion = DateTime.Now,
                    Detalles = detalles
                };

                _context.Devoluciones.Add(devolucion);

                // Restaurar stock
                foreach (var d in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(d.ProductoId);
                    if (producto != null) producto.Stock += d.CantidadDevuelta;
                }

                // Cambiar estado venta a devolucion (id=2)
                var venta = await _context.Ventas.FindAsync(dto.VentaId);
                if (venta != null) venta.EstadoId = 2;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await ObtenerPorId(devolucion.Id) ?? MapDto(devolucion);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DevolucionDto> Gestionar(int id, DevolucionGestionarDto dto)
        {
            var devolucion = await _context.Devoluciones.FindAsync(id)
                ?? throw new Exception("Devolución no encontrada");

            devolucion.EstadoId = dto.NuevoEstado;
            devolucion.UsuarioGestionId = dto.UsuarioGestionId;
            devolucion.FechaGestion = DateTime.Now;

            await _context.SaveChangesAsync();
            return await ObtenerPorId(id) ?? MapDto(devolucion);
        }

        public async Task<List<EstadoDevolucionDto>> ObtenerEstados() =>
            await _context.EstadosDevoluciones
                .Where(e => e.Activo)
                .Select(e => new EstadoDevolucionDto { Id = e.Id, Nombre = e.Nombre, Activo = e.Activo })
                .ToListAsync();
    }
}
