using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface ICompraRepository
    {
        Task<List<CompraDto>> ObtenerTodos();
        Task<CompraDto?> ObtenerPorId(int id);
        Task<CompraDto> Crear(CompraCreateDto dto);
        Task<CompraDto> Actualizar(CompraUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Eliminar(int id);
        Task<List<object>> ObtenerEstados();
    }

    public class CompraRepository : ICompraRepository
    {
        private readonly SyspharmaContext _context;
        public CompraRepository(SyspharmaContext context) => _context = context;

        private static CompraDto MapDto(Compra c) => new CompraDto
        {
            Id = c.Id,
            NumeroCompra = c.NumeroCompra,
            ProveedorId = c.ProveedorId,
            ProveedorNombre = c.Proveedor?.Nombre ?? "",
            ProveedorDocumento = c.Proveedor?.Documento ?? "",
            UsuarioId = c.UsuarioId,
            UsuarioNombre = c.Usuario?.Nombre ?? "",
            EstadoId = c.EstadoId,
            EstadoNombre = c.Estado?.Nombre ?? "",
            Subtotal = c.Subtotal,
            Iva = c.Iva,
            Total = c.Total,
            Notas = c.Notas,
            Observaciones = c.Observaciones,
            FechaCompra = c.FechaCompra,
            FechaEntrega = c.FechaEntrega,
            Detalles = c.CompraDetalles.Select(d => new CompraDetalleDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                ProductoNombre = d.Producto?.Nombre ?? "",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal,
                Lote = d.Lote,
                FechaVencimiento = d.FechaVencimiento
            }).ToList()
        };

        private string GenerarNumeroCompra() =>
            $"COM-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

        public async Task<List<CompraDto>> ObtenerTodos()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.Estado)
                .Include(c => c.CompraDetalles).ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
            return compras.Select(MapDto).ToList();
        }

        public async Task<CompraDto?> ObtenerPorId(int id)
        {
            var c = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.Estado)
                .Include(c => c.CompraDetalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);
            return c == null ? null : MapDto(c);
        }

        public async Task<CompraDto> Crear(CompraCreateDto dto)
        {
            var estado = await _context.EstadosCompras
                .FirstOrDefaultAsync(e => e.Nombre.ToLower() == "pendiente")
                ?? throw new Exception("Estado 'pendiente' no encontrado");

            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new Exception("La compra debe contener al menos un producto.");

            // Validaciones de medicamentos y valores numéricos
            foreach (var det in dto.Detalles)
            {
                if (det.Cantidad <= 0)
                    throw new Exception("La cantidad a comprar debe ser mayor a cero.");
                if (det.PrecioUnitario < 0)
                    throw new Exception("El precio unitario (costo) de compra no puede ser negativo.");

                var prod = await _context.Productos
                    .Include(p => p.ProductoMedicamento)
                    .FirstOrDefaultAsync(p => p.Id == det.ProductoId);

                if (prod != null && prod.ProductoMedicamento != null)
                {
                    if (string.IsNullOrWhiteSpace(det.Lote))
                        throw new Exception($"El lote es obligatorio para el medicamento '{prod.Nombre}'.");
                    if (!det.FechaVencimiento.HasValue)
                        throw new Exception($"La fecha de vencimiento es obligatoria para el medicamento '{prod.Nombre}'.");
                    if (det.FechaVencimiento.Value <= DateOnly.FromDateTime(DateTime.Today))
                        throw new Exception($"La fecha de vencimiento para '{prod.Nombre}' debe ser una fecha futura.");
                }
            }

            var subtotal = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var porcentajeIva = dto.PorcentajeIva ?? 0;
            var iva = Math.Round(subtotal * porcentajeIva / 100, 2);
            var total = subtotal + iva;

            var compra = new Compra
            {
                NumeroCompra = GenerarNumeroCompra(),
                ProveedorId = dto.ProveedorId,
                UsuarioId = dto.UsuarioId,
                EstadoId = estado.Id,
                Subtotal = subtotal,
                Iva = iva,
                Total = total,
                Notas = dto.Notas,
                Observaciones = dto.Observaciones,
                FechaCompra = DateTime.Now,
                FechaEntrega = dto.FechaEntrega,
                CompraDetalles = dto.Detalles.Select(d => new CompraDetalle
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = Math.Round(d.Cantidad * d.PrecioUnitario, 2),
                    Lote = d.Lote,
                    FechaVencimiento = d.FechaVencimiento
                }).ToList()
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();
            return await ObtenerPorId(compra.Id) ?? MapDto(compra);
        }

        public async Task<CompraDto> Actualizar(CompraUpdateDto dto)
        {
            var compra = await _context.Compras
                .Include(c => c.CompraDetalles)
                .FirstOrDefaultAsync(c => c.Id == dto.Id)
                ?? throw new Exception("Compra no encontrada");

            // Validaciones de medicamentos
            foreach (var det in dto.Detalles)
            {
                var prod = await _context.Productos
                    .Include(p => p.ProductoMedicamento)
                    .FirstOrDefaultAsync(p => p.Id == det.ProductoId);

                if (prod != null && prod.ProductoMedicamento != null)
                {
                    if (string.IsNullOrWhiteSpace(det.Lote))
                        throw new Exception($"El lote es obligatorio para el medicamento '{prod.Nombre}'.");
                    if (!det.FechaVencimiento.HasValue)
                        throw new Exception($"La fecha de vencimiento es obligatoria para el medicamento '{prod.Nombre}'.");
                    if (det.FechaVencimiento.Value <= DateOnly.FromDateTime(DateTime.Today))
                        throw new Exception($"La fecha de vencimiento para '{prod.Nombre}' debe ser una fecha futura.");
                }
            }

            compra.ProveedorId = dto.ProveedorId;
            compra.EstadoId = dto.EstadoId;
            compra.Notas = dto.Notas;
            compra.Observaciones = dto.Observaciones;
            compra.FechaEntrega = dto.FechaEntrega;

            // Reemplazar detalles
            _context.CompraDetalles.RemoveRange(compra.CompraDetalles);

            var subtotal = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var iva = Math.Round(subtotal * (compra.Iva > 0 ? compra.Iva / compra.Subtotal : 0), 2);

            compra.Subtotal = subtotal;
            compra.Total = subtotal + iva;
            compra.CompraDetalles = dto.Detalles.Select(d => new CompraDetalle
            {
                CompraId = compra.Id,
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = Math.Round(d.Cantidad * d.PrecioUnitario, 2),
                Lote = d.Lote,
                FechaVencimiento = d.FechaVencimiento
            }).ToList();

            await _context.SaveChangesAsync();
            return await ObtenerPorId(compra.Id) ?? MapDto(compra);
        }

        public async Task<bool> CambiarEstado(int id, int estadoId)
        {
            var c = await _context.Compras
                .Include(x => x.CompraDetalles)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return false;

            c.EstadoId = estadoId;

            var estadoRecibida = await _context.EstadosCompras.FirstOrDefaultAsync(e => e.Nombre.ToLower() == "recibida");
            if (estadoRecibida != null && estadoId == estadoRecibida.Id)
            {
                // Crear lotes si no han sido creados previamente
                if (!await _context.Lotes.AnyAsync(l => l.CompraId == id))
                {
                    foreach (var det in c.CompraDetalles)
                    {
                        var lote = new Lote
                        {
                            ProductoId = det.ProductoId,
                            CompraId = c.Id,
                            NumeroLote = det.Lote ?? $"LOTE-{c.NumeroCompra}-{det.ProductoId}",
                            Cantidad = det.Cantidad,
                            FechaVencimiento = det.FechaVencimiento ?? DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
                            CostoUnitario = det.PrecioUnitario,
                            FechaCreacion = DateTime.Now
                        };
                        _context.Lotes.Add(lote);

                        // Incrementar el stock global del producto
                        var prod = await _context.Productos.FindAsync(det.ProductoId);
                        if (prod != null)
                        {
                            prod.Stock += det.Cantidad;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.CompraDetalles)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (compra == null) return false;

            _context.CompraDetalles.RemoveRange(compra.CompraDetalles);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<object>> ObtenerEstados() =>
            await _context.EstadosCompras.Select(e => (object)new { e.Id, e.Nombre }).ToListAsync();
    }
}
