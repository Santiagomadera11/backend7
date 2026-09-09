using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IVentaService
    {
        Task<List<VentaDto>> ObtenerTodos();
        Task<VentaDto?> ObtenerPorId(int id);
        Task<VentaDto> Crear(VentaCreateDto dto);
        Task<VentaDto> Actualizar(VentaUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<List<EstadoVentaDto>> ObtenerEstados();
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Anular(int id);
        Task<VentaDto> CrearDesdePedido(int pedidoId);
    }

    public class VentaService : IVentaService
    {
        private readonly SyspharmaContext _context;
        private readonly IMapper _mapper;

        public VentaService(SyspharmaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<VentaDto> CrearDesdePedido(int pedidoId)
        {
            // 1. Cargar el pedido con sus detalles
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.MetodoPago)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == pedidoId)
                ?? throw new Exception($"Pedido con ID {pedidoId} no encontrado.");

            // 2. Buscar el turno activo
            // El turno activo es el que tiene estado = "activo" más reciente.
            var turnoActivo = await _context.Turnos
                .Where(t => t.Estado == "activo")
                .OrderByDescending(t => t.FechaApertura)
                .FirstOrDefaultAsync()
                ?? throw new Exception(
                    "No hay un turno (caja) activo. Abrí un turno antes de marcar el pedido como Entregado.");

            // 3. Buscar estado "Completada" en ventas
            var estadoCompletada = await _context.EstadosVenta
                .FirstOrDefaultAsync(e => e.Nombre == "Completada")
                ?? throw new Exception("No se encontró el estado 'Completada' en la tabla estados_venta.");


            int metodoPagoId;
            if (pedido.MetodoPagoId.HasValue)
            {
                metodoPagoId = pedido.MetodoPagoId.Value;
            }
            else
            {
                var metodoPorDefecto = await _context.MetodosPagos
                    .Where(m => m.Estado == true)
                    .OrderBy(m => m.Id)
                    .FirstOrDefaultAsync()
                    ?? throw new Exception("No hay métodos de pago disponibles en el sistema.");

                metodoPagoId = metodoPorDefecto.Id;
            }

            // 5. Validar que haya detalles con ProductoId (no nulo)
            var detallesValidos = pedido.PedidoDetalles
                .Where(d => d.ProductoId.HasValue)
                .ToList();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 6. Crear la Venta
                var venta = new Venta
                {
                    NumeroVenta = $"VNT-{DateTime.Now:yyyyMMddHHmmss}-P{pedido.Id}",
                    TurnoId = turnoActivo.Id,
                    // UsuarioId: usar el del pedido si existe, si no el que abrió el turno
                    UsuarioId = pedido.UsuarioId ?? turnoActivo.UsuarioId,
                    ClienteNombre = string.IsNullOrWhiteSpace(pedido.ClienteNombre)
                        ? "Consumidor Final"
                        : pedido.ClienteNombre,
                    ClienteDocumento = pedido.ClienteDocumento,
                    ClienteTelefono = pedido.ClienteTelefono,
                    MetodoPagoId = metodoPagoId,
                    EstadoId = estadoCompletada.Id,
                    Subtotal = pedido.Subtotal,
                    Iva = pedido.Iva,
                    PorcentajeIva = 0, // ajustá si tu Pedido guarda el porcentaje
                    Total = pedido.Total,
                    Notas = $"Generada automáticamente desde pedido {pedido.NumeroPedido}",
                    FechaVenta = DateTime.Now,
                    Origen = "WEB",
                    PedidoId = pedido.Id
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync(); // necesitamos el Id de la venta

                // 7. Copiar los detalles de PedidoDetalle → VentaDetalle
                foreach (var detalle in detallesValidos)
                {
                    // Validar stock
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId!.Value);
                    if (producto != null)
                    {
                        if (producto.Stock < detalle.Cantidad)
                            throw new Exception(
                                $"Stock insuficiente para '{producto.Nombre}'. " +
                                $"Disponible: {producto.Stock}, requerido: {detalle.Cantidad}.");

                        producto.Stock -= detalle.Cantidad;
                        producto.UltimaActualizacion = DateTime.Now;
                    }

                    _context.VentaDetalles.Add(new VentaDetalle
                    {
                        VentaId = venta.Id,
                        ProductoId = detalle.ProductoId!.Value,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Descuento = 0,
                        Subtotal = detalle.Subtotal
                    });
                }

                // 8. Actualizar totales del turno
                turnoActivo.TotalVentas += pedido.Total;
                turnoActivo.ResumenVentas += 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await ObtenerPorId(venta.Id)
                    ?? _mapper.Map<VentaDto>(venta);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<List<VentaDto>> ObtenerTodos()
        {
            try
            {
                var ventas = await _context.Ventas
                    .Include(v => v.Estado)
                    .Include(v => v.MetodoPago)
                    .Include(v => v.Usuario)
                    .Include(v => v.VentaDetalles).ThenInclude(d => d.Producto)
                    .Include(v => v.VentaDetallesServicios).ThenInclude(s => s.Servicio)
                    .OrderByDescending(v => v.FechaVenta)
                    .ToListAsync();

                return _mapper.Map<List<VentaDto>>(ventas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerTodos: {ex.Message}");
                return new List<VentaDto>();
            }
        }

        public async Task<VentaDto?> ObtenerPorId(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Estado)
                .Include(v => v.MetodoPago)
                .Include(v => v.Usuario)
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Producto)
                .Include(v => v.VentaDetallesServicios).ThenInclude(s => s.Servicio)
                .FirstOrDefaultAsync(v => v.Id == id);

            return _mapper.Map<VentaDto>(venta);
        }

        public async Task<VentaDto> Crear(VentaCreateDto dto)
        {
            if (dto.TurnoId <= 0)
                throw new Exception("No se puede crear la venta: El ID de Turno no es válido (0). Asegúrese de tener una caja abierta.");

            var turno = await _context.Turnos.FindAsync(dto.TurnoId);
            if (turno == null)
                throw new Exception($"El turno con ID {dto.TurnoId} no existe en la base de datos. Por favor, cierre sesión y vuelva a entrar.");

            if (turno.Estado != "activo")
                throw new Exception("El turno (caja) seleccionado no está activo o ya ha sido cerrado. Por favor, abra un turno de caja antes de registrar ventas.");

            var metodoPago = await _context.MetodosPagos.FindAsync(dto.MetodoPagoId);
            if (metodoPago == null)
                throw new Exception("El método de pago seleccionado no es válido o no existe.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal subtotalProd = dto.Detalles?.Sum(d => (d.Cantidad * d.PrecioUnitario) - d.Descuento) ?? 0;
                decimal subtotalServ = dto.Servicios?.Sum(s => (s.Cantidad * s.PrecioUnitario) - s.Descuento) ?? 0;
                decimal subtotalFinal = subtotalProd + subtotalServ;
                decimal porcentajeIva = dto.PorcentajeIva > 0 ? dto.PorcentajeIva : 0;
                decimal ivaFinal = Math.Round(subtotalFinal * (porcentajeIva / 100), 2);
                decimal totalFinal = subtotalFinal + ivaFinal;

                var venta = new Venta
                {
                    NumeroVenta = $"VNT-{DateTime.Now:yyyyMMddHHmmss}",
                    TurnoId = dto.TurnoId,
                    UsuarioId = dto.UsuarioId,
                    ClienteNombre = string.IsNullOrWhiteSpace(dto.ClienteNombre) ? "Consumidor Final" : dto.ClienteNombre,
                    ClienteDocumento = dto.ClienteDocumento,
                    ClienteTelefono = dto.ClienteTelefono,
                    MetodoPagoId = dto.MetodoPagoId,
                    EstadoId = 1,
                    Subtotal = subtotalFinal,
                    PorcentajeIva = porcentajeIva,
                    Notas = dto.Notas,
                    FechaVenta = DateTime.Now,
                    Origen = string.IsNullOrWhiteSpace(dto.Origen) ? "CAJA" : dto.Origen,
                    PedidoId = dto.PedidoId,
                    ReferenciasPago = dto.ReferenciasPago
                };

                // ✔ Usar la propiedad enviada por el cliente para calcular IVA
                venta.Iva = venta.Subtotal * (venta.PorcentajeIva / 100.0m);
                venta.Total = venta.Subtotal + venta.Iva;

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                if (dto.Detalles != null && dto.Detalles.Any())
                {
                    foreach (var d in dto.Detalles)
                    {
                        if (d.Cantidad <= 0)
                            throw new Exception("La cantidad del producto vendido debe ser mayor a cero.");
                        if (d.PrecioUnitario < 0)
                            throw new Exception("El precio unitario del producto no puede ser negativo.");
                        if (d.Descuento < 0 || d.Descuento > d.Cantidad * d.PrecioUnitario)
                            throw new Exception("El descuento no puede ser negativo ni mayor al subtotal del producto.");

                        _context.VentaDetalles.Add(new VentaDetalle
                        {
                            VentaId = venta.Id,
                            ProductoId = d.ProductoId,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Descuento = d.Descuento,
                            Subtotal = (d.Cantidad * d.PrecioUnitario) - d.Descuento,
                            LoteId = d.LoteId
                        });

                        var producto = await _context.Productos.FindAsync(d.ProductoId);
                        if (producto != null)
                        {
                            if (producto.Stock < d.Cantidad)
                                throw new Exception($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {d.Cantidad}.");
                            producto.Stock -= d.Cantidad;
                            producto.UltimaActualizacion = DateTime.Now;
                        }

                        if (d.LoteId.HasValue && d.LoteId.Value > 0)
                        {
                            var lote = await _context.Lotes.FindAsync(d.LoteId.Value);
                            if (lote != null)
                            {
                                if (lote.Cantidad < d.Cantidad)
                                    throw new Exception($"Stock insuficiente en el lote '{lote.NumeroLote}' para '{producto?.Nombre ?? "Producto"}'. Disponible: {lote.Cantidad}, solicitado: {d.Cantidad}.");
                                lote.Cantidad -= d.Cantidad;
                            }
                        }
                    }
                }

                if (dto.Servicios != null && dto.Servicios.Any())
                {
                    foreach (var s in dto.Servicios)
                    {
                        if (s.Cantidad <= 0)
                            throw new Exception("La cantidad del servicio vendido debe ser mayor a cero.");
                        if (s.PrecioUnitario < 0)
                            throw new Exception("El precio unitario del servicio no puede ser negativo.");
                        if (s.Descuento < 0 || s.Descuento > s.Cantidad * s.PrecioUnitario)
                            throw new Exception("El descuento del servicio no puede ser negativo ni mayor al subtotal del servicio.");

                        _context.VentaDetalleServicios.Add(new VentaDetalleServicio
                        {
                            VentaId = venta.Id,
                            ServicioId = s.ServicioId,
                            Cantidad = s.Cantidad,
                            PrecioUnitario = s.PrecioUnitario,
                            Descuento = s.Descuento,
                            Subtotal = (s.Cantidad * s.PrecioUnitario) - s.Descuento
                        });
                    }
                }

                turno.TotalVentas += totalFinal;
                turno.ResumenVentas += 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await ObtenerPorId(venta.Id) ?? _mapper.Map<VentaDto>(venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                string innerMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                throw new Exception($"Error al procesar la venta: {ex.Message}. {innerMessage}");
            }
        }

        public async Task<VentaDto> Actualizar(VentaUpdateDto dto)
        {
            var venta = await _context.Ventas.FindAsync(dto.Id);
            if (venta == null) throw new Exception("La venta no existe.");
            _mapper.Map(dto, venta);
            await _context.SaveChangesAsync();
            return await ObtenerPorId(venta.Id) ?? _mapper.Map<VentaDto>(venta);
        }


        public async Task<bool> Eliminar(int id)
        {
            var v = await _context.Ventas
                .Include(v => v.VentaDetalles)
                .Include(v => v.VentaDetallesServicios)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (v == null) return false;
            _context.VentaDetalles.RemoveRange(v.VentaDetalles);
            _context.VentaDetalleServicios.RemoveRange(v.VentaDetallesServicios);
            _context.Ventas.Remove(v);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EstadoVentaDto>> ObtenerEstados()
        {
            var estados = await _context.EstadosVenta.ToListAsync();
            return _mapper.Map<List<EstadoVentaDto>>(estados);
        }

        public async Task<bool> CambiarEstado(int id, int estadoId)
        {
            var v = await _context.Ventas.FindAsync(id);
            if (v == null) return false;
            v.EstadoId = estadoId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Anular(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.VentaDetalles)
                .Include(v => v.Turno)
                .FirstOrDefaultAsync(v => v.Id == id)
                ?? throw new Exception("La venta no existe.");

            if (venta.EstadoId == 3)
                throw new Exception("La venta ya está anulada.");

            if (venta.EstadoId == 2)
                throw new Exception("No se puede anular una venta con devolución aprobada.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cambiar estado a anulada (3)
                venta.EstadoId = 3;

                // 2. Devolver stock de cada producto
                foreach (var detalle in venta.VentaDetalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                        producto.UltimaActualizacion = DateTime.Now;
                    }
                }

                // 3. Restar del turno
                if (venta.Turno != null)
                {
                    venta.Turno.TotalVentas -= venta.Total;
                    if (venta.Turno.ResumenVentas > 0)
                        venta.Turno.ResumenVentas -= 1;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                string innerMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                throw new Exception($"Error al anular la venta: {ex.Message}. {innerMessage}");
            }
        }
    }
}
