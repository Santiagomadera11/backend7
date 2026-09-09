using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syspharma.Data.Repositories
{
    public interface IPedidoRepository
    {
        Task<List<PedidoDto>> ObtenerTodos();
        Task<PedidoDto?> ObtenerPorId(int id);
        Task<List<PedidoDto>> ObtenerPorUsuario(int usuarioId);
        Task<PedidoDto> Crear(PedidoCreateDto dto);
        Task<PedidoDto> Actualizar(PedidoUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Eliminar(int id);
        Task<List<object>> ObtenerEstados();
    }

    public class PedidoRepository : IPedidoRepository
    {
        private readonly SyspharmaContext _context;
        public PedidoRepository(SyspharmaContext context) => _context = context;

        private static PedidoDto MapDto(Pedido p)
        {
            var detallesList = p.PedidoDetalles.Select(d =>
            {
                var porcentajeIva = d.Producto?.PorcentajeIva ?? 0;
                var ivaLinea = d.Subtotal * (porcentajeIva / 100);
                return new PedidoDetalleDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Nombre = d.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    PorcentajeIva = porcentajeIva,
                    Iva = ivaLinea
                };
            }).ToList();

            var sumaIvas = detallesList.Sum(d => d.Iva);
            var ivaConsistente = Math.Abs(sumaIvas - p.Iva) < 0.1m;

            return new PedidoDto
            {
                Id = p.Id,
                NumeroPedido = p.NumeroPedido,
                UsuarioId = p.UsuarioId,
                UsuarioNombre = p.Usuario?.Nombre ?? "Cliente Web",
                ClienteNombre = p.ClienteNombre,
                ClienteDocumento = p.ClienteDocumento,
                ClienteTelefono = p.ClienteTelefono,
                ClienteEmail = p.ClienteEmail,
                Direccion = p.Direccion,
                MetodoPagoId = p.MetodoPagoId,
                MetodoPagoNombre = p.MetodoPago?.Nombre ?? "No definido",
                EstadoId = p.EstadoId,
                EstadoNombre = p.Estado?.Nombre ?? "Pendiente",
                Subtotal = p.Subtotal,
                Iva = p.Iva,
                Total = p.Total,
                Notas = p.Notas,
                Origen = p.Origen,
                FechaCreacion = p.FechaCreacion,
                FechaEntrega = p.FechaEntrega,
                IvaConsistente = ivaConsistente,
                Detalles = detallesList
            };
        }

        private string GenerarNumeroPedido() => $"PED-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

        public async Task<List<PedidoDto>> ObtenerTodos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario).Include(p => p.MetodoPago).Include(p => p.Estado).Include(p => p.PedidoDetalles).ThenInclude(d => d.Producto)
                .OrderByDescending(p => p.FechaCreacion).ToListAsync();
            return pedidos.Select(MapDto).ToList();
        }

        public async Task<List<PedidoDto>> ObtenerPorUsuario(int usuarioId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario).Include(p => p.Estado).Include(p => p.PedidoDetalles).ThenInclude(d => d.Producto)
                .Where(p => p.UsuarioId == usuarioId).OrderByDescending(p => p.FechaCreacion).ToListAsync();
            return pedidos.Select(MapDto).ToList();
        }

        public async Task<PedidoDto?> ObtenerPorId(int id)
        {
            var p = await _context.Pedidos
                .Include(p => p.Usuario).Include(p => p.MetodoPago).Include(p => p.Estado).Include(p => p.PedidoDetalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);
            return p == null ? null : MapDto(p);
        }

        public async Task<PedidoDto> Crear(PedidoCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var estadoPendiente = await _context.EstadosPedidos.FirstOrDefaultAsync(e => e.Nombre == "Pendiente")
                    ?? throw new Exception("Estado inicial 'Pendiente' no encontrado.");

                // VALIDACIÓN DE MÉTODO DE PAGO (Evita el error 400)
                int idMetodoPago = (dto.MetodoPagoId.HasValue && dto.MetodoPagoId > 0) ? dto.MetodoPagoId.Value : 1;

                // Cargar los IVA reales de los productos desde la base de datos
                var productoIds = dto.Detalles?.Select(d => d.ProductoId).Where(id => id.HasValue).Select(id => id.Value).ToList() ?? new List<int>();
                var productosDb = await _context.Productos
                    .Where(p => productoIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p.PorcentajeIva);

                decimal subtotal = 0;
                decimal totalIva = 0;

                if (dto.Detalles != null)
                {
                    foreach (var d in dto.Detalles)
                    {
                        var subtotalLinea = d.Cantidad * d.PrecioUnitario;
                        subtotal += subtotalLinea;
                        decimal porcentajeIva = 0;
                        if (d.ProductoId.HasValue && productosDb.TryGetValue(d.ProductoId.Value, out var pctIva))
                        {
                            porcentajeIva = pctIva;
                        }
                        totalIva += subtotalLinea * (porcentajeIva / 100);
                    }
                }

                decimal iva = totalIva;

                var pedido = new Pedido
                {
                    NumeroPedido = GenerarNumeroPedido(),
                    // Forzamos la lectura del DTO
                    UsuarioId = dto.UsuarioId,
                    ClienteNombre = dto.ClienteNombre,
                    ClienteEmail = dto.ClienteEmail,
                    Direccion = dto.Direccion,
                    ClienteDocumento = dto.ClienteDocumento,
                    ClienteTelefono = dto.ClienteTelefono,
                    MetodoPagoId = idMetodoPago,
                    EstadoId = estadoPendiente.Id,
                    Subtotal = subtotal,
                    Iva = iva,
                    Total = subtotal + iva,
                    Notas = dto.Notas ?? "Pedido realizado desde la web",
                    Origen = "web",
                    FechaCreacion = DateTime.Now,
                    FechaEntrega = dto.FechaEntrega
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                if (dto.Detalles == null || !dto.Detalles.Any())
                {
                    throw new Exception("El pedido debe contener al menos un producto.");
                }

                foreach (var d in dto.Detalles)
                {
                    if (d.Cantidad <= 0)
                        throw new Exception($"La cantidad solicitada para el producto '{d.Nombre}' debe ser mayor a cero.");
                    if (d.PrecioUnitario < 0)
                        throw new Exception($"El precio unitario para el producto '{d.Nombre}' no puede ser negativo.");

                    _context.PedidoDetalles.Add(new PedidoDetalle
                    {
                        PedidoId = pedido.Id,
                        ProductoId = d.ProductoId,
                        Nombre = d.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario
                    });

                    var producto = await _context.Productos.FindAsync(d.ProductoId);
                    if (producto != null)
                    {
                        if (producto.Stock < d.Cantidad)
                            throw new Exception($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {d.Cantidad}.");
                        producto.Stock -= d.Cantidad;
                        _context.Entry(producto).State = EntityState.Modified;
                    }
                }

                // Link Citas to the created Pedido
                if (dto.CitaIds != null && dto.CitaIds.Any())
                {
                    foreach (var citaId in dto.CitaIds)
                    {
                        var cita = await _context.Citas.FindAsync(citaId);
                        if (cita != null)
                        {
                            cita.PedidoId = pedido.Id;
                            var estadoPagada = await _context.EstadosCita.FirstOrDefaultAsync(e => e.Nombre == "Pagada");
                            if (estadoPagada != null)
                            {
                                cita.EstadoId = estadoPagada.Id;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await ObtenerPorId(pedido.Id) ?? MapDto(pedido);
            }
            catch (Exception ex) { await transaction.RollbackAsync(); throw new Exception(ex.Message); }
        }

        public async Task<PedidoDto> Actualizar(PedidoUpdateDto dto)
        {
            var p = await _context.Pedidos.FindAsync(dto.Id) ?? throw new Exception("No encontrado");
            p.ClienteNombre = dto.ClienteNombre; p.EstadoId = dto.EstadoId;
            await _context.SaveChangesAsync();
            return await ObtenerPorId(p.Id) ?? throw new Exception("Error");
        }

        public async Task<bool> CambiarEstado(int id, int estadoId)
        {
            var p = await _context.Pedidos.FindAsync(id);
            if (p == null) return false;
            p.EstadoId = estadoId;

            // Check if status is updated to Entregado
            var estado = await _context.EstadosPedidos.FindAsync(estadoId);
            if (estado != null && estado.Nombre == "Entregado")
            {
                var existeVenta = await _context.Ventas.AnyAsync(v => v.PedidoId == id);
                if (!existeVenta)
                {
                    // Find active turn or fallback
                    var turnoActivo = await _context.Turnos
                        .Where(t => t.Estado.Contains("activo"))
                        .OrderByDescending(t => t.Id)
                        .FirstOrDefaultAsync();
                    int turnoId = turnoActivo?.Id ?? 0;
                    if (turnoId == 0)
                    {
                        var ultimoTurno = await _context.Turnos
                            .OrderByDescending(t => t.Id)
                            .FirstOrDefaultAsync();
                        turnoId = ultimoTurno?.Id ?? 1;
                    }

                    var orderWithDetails = await _context.Pedidos
                        .Include(o => o.PedidoDetalles)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (orderWithDetails != null)
                    {
                        var venta = new Venta
                        {
                            NumeroVenta = $"VNT-WEB-{DateTime.Now:yyyyMMddHHmmss}",
                            TurnoId = turnoId,
                            UsuarioId = orderWithDetails.UsuarioId ?? 1,
                            ClienteNombre = orderWithDetails.ClienteNombre,
                            ClienteDocumento = orderWithDetails.ClienteDocumento,
                            ClienteTelefono = orderWithDetails.ClienteTelefono,
                            MetodoPagoId = orderWithDetails.MetodoPagoId ?? 1,
                            EstadoId = 1, // 1 = Completada
                            Subtotal = orderWithDetails.Subtotal,
                            Iva = orderWithDetails.Iva,
                            PorcentajeIva = orderWithDetails.Subtotal > 0 ? Math.Round((orderWithDetails.Iva / orderWithDetails.Subtotal) * 100, 2) : 19,
                            Total = orderWithDetails.Total,
                            Notas = $"Pedido #{orderWithDetails.NumeroPedido}. {orderWithDetails.Notas}",
                            FechaVenta = DateTime.Now,
                            Origen = "WEB",
                            PedidoId = orderWithDetails.Id
                        };
                        _context.Ventas.Add(venta);
                        await _context.SaveChangesAsync();

                        foreach (var pd in orderWithDetails.PedidoDetalles)
                        {
                            var vd = new VentaDetalle
                            {
                                VentaId = venta.Id,
                                ProductoId = pd.ProductoId ?? 0,
                                Cantidad = pd.Cantidad,
                                PrecioUnitario = pd.PrecioUnitario,
                                Descuento = 0,
                                Subtotal = pd.Subtotal
                            };
                            _context.VentaDetalles.Add(vd);
                        }

                        var turno = await _context.Turnos.FindAsync(turnoId);
                        if (turno != null)
                        {
                            turno.TotalVentas += venta.Total;
                            turno.ResumenVentas += 1;
                        }

                        // Update VentaId on any Citas linked to this Pedido
                        var citasAsociadas = await _context.Citas.Where(c => c.PedidoId == id).ToListAsync();
                        foreach (var cita in citasAsociadas)
                        {
                            cita.VentaId = venta.Id;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var p = await _context.Pedidos.Include(p => p.PedidoDetalles).FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return false;
            _context.PedidoDetalles.RemoveRange(p.PedidoDetalles);
            _context.Pedidos.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<object>> ObtenerEstados() => await _context.EstadosPedidos.Select(e => (object)new { e.Id, e.Nombre }).ToListAsync();
    }
}