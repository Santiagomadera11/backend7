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
        public async Task<List<VentaDto>> ObtenerTodos()
        {
            try
            {
                var ventas = await _context.Ventas
                    .Include(v => v.Estado)
                    .Include(v => v.MetodoPago)
                    .Include(v => v.Usuario)
                    .Include(v => v.VentaDetalles).ThenInclude(d => d.Producto)
                    .Include(v => v.VentaDetalles).ThenInclude(d => d.Lote)
                    .Include(v => v.VentaDetalles).ThenInclude(d => d.Lotes).ThenInclude(vdl => vdl.Lote)
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
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Lote)
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Lotes).ThenInclude(vdl => vdl.Lote)
                .Include(v => v.VentaDetallesServicios).ThenInclude(s => s.Servicio)
                .FirstOrDefaultAsync(v => v.Id == id);

            return _mapper.Map<VentaDto>(venta);
        }

        public async Task<VentaDto> Crear(VentaCreateDto dto)
        {
            // Los empleados necesitan caja/turno abierta para vender (así se audita el
            // efectivo que manejan). Los administradores no: pueden vender sin turno
            // asociado, esa venta simplemente no entra en ningún cuadre de caja.
            var usuario = await _context.Usuarios.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == dto.UsuarioId);
            var esAdministrador = usuario != null && usuario.Role.Nombre == "Administrador";

            Turno? turno = null;
            if (dto.TurnoId.HasValue && dto.TurnoId.Value > 0)
            {
                turno = await _context.Turnos.FindAsync(dto.TurnoId.Value);
                if (turno == null)
                    throw new Exception($"El turno con ID {dto.TurnoId} no existe en la base de datos. Por favor, cierre sesión y vuelva a entrar.");
                if (turno.Estado != "activo")
                    throw new Exception("El turno (caja) seleccionado no está activo o ya ha sido cerrado. Por favor, abra un turno de caja antes de registrar ventas.");
            }
            else if (!esAdministrador)
            {
                throw new Exception("No se puede crear la venta: El ID de Turno no es válido (0). Asegúrese de tener una caja abierta.");
            }

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

                // El cuadre de caja (Turno.TotalVentas / saldoEsperado) cuenta SOLO productos:
                // el dinero de servicios/citas cobrado en esta misma venta no se espera acá,
                // se audita aparte en el panel de Citas ("Ingresos por Citas Hoy"). A pedido
                // del negocio, aunque esa plata sí entra físicamente al mismo cajón.
                decimal ivaProd = Math.Round(subtotalProd * (porcentajeIva / 100), 2);
                decimal totalProductos = subtotalProd + ivaProd;

                var venta = new Venta
                {
                    NumeroVenta = $"VNT-{DateTime.Now:yyyyMMddHHmmss}",
                    TurnoId = turno?.Id,
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

                        // Resolver la forma de venta (Unidad/Blister/Caja) para calcular el
                        // factor de conversión real a unidades. FormaVentaId es opcional
                        // (compatibilidad con llamadas que no lo mandan): en ese caso factor = 1.
                        Syspharma.Data.Entities.ProductoFormaVenta? formaVenta = null;
                        int factor = 1;
                        if (d.FormaVentaId.HasValue)
                        {
                            formaVenta = await _context.ProductoFormasVenta
                                .FirstOrDefaultAsync(f => f.Id == d.FormaVentaId.Value && f.ProductoId == d.ProductoId);
                            if (formaVenta == null || !formaVenta.Activo)
                                throw new Exception($"La forma de venta seleccionada para el producto no existe o fue deshabilitada.");
                            factor = formaVenta.FactorUnidades;
                        }

                        var unidadesADescontar = d.Cantidad * factor;

                        var producto = await _context.Productos.FindAsync(d.ProductoId);
                        if (producto != null)
                        {
                            if (producto.Stock < unidadesADescontar)
                                throw new Exception($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {unidadesADescontar}.");
                            producto.Stock -= unidadesADescontar;
                            producto.UltimaActualizacion = DateTime.Now;
                        }

                        // Si el cliente indicó un lote específico, se respeta esa elección.
                        // Si no, se aplica FEFO (First-Expired-First-Out): se descuenta primero
                        // de los lotes que vencen antes, para que el producto próximo a vencer
                        // salga primero del inventario. Cada lote realmente tocado queda
                        // registrado en VentaDetalleLote con su cantidad exacta, para poder
                        // revertir la venta con precisión (una sola línea puede repartirse
                        // entre varios lotes).
                        var detalleEntity = new VentaDetalle
                        {
                            VentaId = venta.Id,
                            ProductoId = d.ProductoId,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Descuento = d.Descuento,
                            Subtotal = (d.Cantidad * d.PrecioUnitario) - d.Descuento,
                            FormaVentaId = formaVenta?.Id,
                            FormaVentaTipo = formaVenta?.Tipo,
                            FactorUnidades = factor
                        };

                        int? loteIdAsignado = d.LoteId;

                        var hoyDateOnly = DateOnly.FromDateTime(DateTime.Today);

                        if (loteIdAsignado.HasValue && loteIdAsignado.Value > 0)
                        {
                            var lote = await _context.Lotes.FindAsync(loteIdAsignado.Value);
                            if (lote != null)
                            {
                                if (lote.FechaVencimiento < hoyDateOnly)
                                    throw new Exception($"No se puede vender del lote '{lote.NumeroLote}' de '{producto?.Nombre ?? "Producto"}': está vencido desde {lote.FechaVencimiento:yyyy-MM-dd}.");
                                if (lote.Cantidad < unidadesADescontar)
                                    throw new Exception($"Stock insuficiente en el lote '{lote.NumeroLote}' para '{producto?.Nombre ?? "Producto"}'. Disponible: {lote.Cantidad}, solicitado: {unidadesADescontar}.");
                                lote.Cantidad -= unidadesADescontar;
                                detalleEntity.Lotes.Add(new VentaDetalleLote { LoteId = lote.Id, Cantidad = unidadesADescontar });
                            }
                        }
                        else
                        {
                            var lotesActivos = await _context.Lotes
                                .Where(l => l.ProductoId == d.ProductoId
                                            && l.Cantidad > 0
                                            && l.FechaVencimiento >= hoyDateOnly)
                                .OrderBy(l => l.FechaVencimiento)
                                .ToListAsync();

                            var pendiente = unidadesADescontar;
                            foreach (var lote in lotesActivos)
                            {
                                if (pendiente <= 0) break;
                                var aDescontar = Math.Min(lote.Cantidad, pendiente);
                                lote.Cantidad -= aDescontar;
                                pendiente -= aDescontar;
                                detalleEntity.Lotes.Add(new VentaDetalleLote { LoteId = lote.Id, Cantidad = aDescontar });
                                loteIdAsignado ??= lote.Id;
                            }

                            if (pendiente > 0)
                            {
                                // Si el producto no tiene NINGÚN lote registrado (no es
                                // medicamento, o nunca se trazó por lote), no hay nada contra
                                // qué comparar: el contador plano Producto.Stock manda solo.
                                // Pero si SÍ tiene lotes y aun así no alcanzó lo vigente, la
                                // diferencia son unidades vencidas — no se pueden vender.
                                var tieneLotesRegistrados = await _context.Lotes.AnyAsync(l => l.ProductoId == d.ProductoId);
                                if (tieneLotesRegistrados)
                                    throw new Exception($"No se puede vender '{producto?.Nombre ?? "el producto"}': faltan {pendiente} unidades de stock vigente (no vencido). El resto del inventario registrado está vencido.");
                            }
                        }

                        // LoteId queda como referencia rápida al primer lote tocado (para
                        // mostrar en pantalla); la fuente de verdad para revertir es Lotes.
                        detalleEntity.LoteId = loteIdAsignado;

                        _context.VentaDetalles.Add(detalleEntity);
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
                            Subtotal = (s.Cantidad * s.PrecioUnitario) - s.Descuento,
                            CitaId = s.CitaId
                        });

                        // Si el servicio vendido corresponde a una cita agendada, marcarla
                        // como pagada y vincularla a esta venta. Este paso existía en el
                        // repositorio de ventas viejo (sin usar en el flujo real) pero nunca
                        // se portó a este servicio, que es el que realmente procesa las
                        // ventas: las citas cobradas se quedaban para siempre en su estado
                        // anterior (pendiente/confirmada) sin registrar el pago.
                        if (s.CitaId.HasValue && s.CitaId.Value > 0)
                        {
                            var cita = await _context.Citas.FindAsync(s.CitaId.Value);
                            if (cita != null)
                            {
                                cita.VentaId = venta.Id;
                                var estadoPagada = await _context.EstadosCita.FirstOrDefaultAsync(e => e.Nombre == "Pagada");
                                if (estadoPagada != null)
                                    cita.EstadoId = estadoPagada.Id;
                            }
                        }
                    }
                }

                // Sin turno (venta de administrador sin caja abierta), no hay nada que
                // cuadrar: esta venta no se refleja en ningún saldoEsperado.
                if (turno != null)
                {
                    turno.TotalVentas += totalProductos;
                    turno.ResumenVentas += 1;
                }

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

            // Solo se puede borrar físicamente una venta que ya fue anulada: Anular() ya
            // revirtió su stock, sus lotes y el total del turno correctamente. Borrar una
            // venta activa destruiría el registro sin devolver nada de eso, perdiendo esas
            // unidades del inventario para siempre y sin dejar ningún rastro de qué pasó.
            if (v.EstadoId != 3)
                throw new Exception("Solo se puede eliminar una venta que ya fue anulada. Anúlala primero para revertir su stock, sus lotes y su efecto en el turno.");

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

            // "Anulada" (3) y "Devolución" (2) tienen efectos reales de stock/lotes que
            // solo Anular() y el módulo de Devoluciones saben aplicar correctamente
            // (reversión de Producto.Stock y Lote.Cantidad). Cambiar el estado acá
            // directamente saltaría esa lógica y dejaría el inventario desincronizado.
            if (estadoId == 3 || v.EstadoId == 3)
                throw new Exception("Para anular o reactivar una venta anulada, usa el endpoint de anulación, no este.");
            if (estadoId == 2 || v.EstadoId == 2)
                throw new Exception("El estado 'Devolución' solo cambia a través del módulo de Devoluciones.");

            v.EstadoId = estadoId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Anular(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Lotes)
                .Include(v => v.VentaDetallesServicios)
                .Include(v => v.Turno)
                .FirstOrDefaultAsync(v => v.Id == id)
                ?? throw new Exception("La venta no existe.");

            if (venta.EstadoId == 3)
                throw new Exception("La venta ya está anulada.");

            if (venta.EstadoId == 2)
                throw new Exception("No se puede anular una venta con devolución aprobada.");

            // Si el turno de esta venta ya se cerró, su "Diferencia" (cuadre de caja) ya
            // quedó calculada y auditada con el TotalVentas de ese momento. Anular acá
            // restaría del total sin recalcular esa diferencia, dejando el cuadre histórico
            // desactualizado sin que nadie se entere.
            if (venta.Turno != null && venta.Turno.Estado.Contains("cerrado"))
                throw new Exception("No se puede anular esta venta: pertenece a un turno que ya fue cerrado y su cuadre de caja ya quedó registrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cambiar estado a anulada (3)
                venta.EstadoId = 3;

                // 2. Devolver stock de cada producto. Se usa el FactorUnidades YA CONGELADO
                // en la fila VentaDetalle histórica (no se recalcula buscando la forma de
                // venta actual, que puede haber cambiado o estar deshabilitada).
                foreach (var detalle in venta.VentaDetalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad * detalle.FactorUnidades;
                        producto.UltimaActualizacion = DateTime.Now;
                    }

                    if (detalle.Lotes != null && detalle.Lotes.Any())
                    {
                        // Fuente de verdad: restaura exactamente lo que salió de cada lote,
                        // incluso si la línea se repartió entre varios.
                        foreach (var detalleLote in detalle.Lotes)
                        {
                            var lote = await _context.Lotes.FindAsync(detalleLote.LoteId);
                            if (lote != null)
                                lote.Cantidad += detalleLote.Cantidad;
                        }
                    }
                    else if (detalle.LoteId.HasValue)
                    {
                        // Compatibilidad con ventas registradas antes de este cambio, que solo
                        // guardaban un LoteId único sin desglose por VentaDetalleLote.
                        var lote = await _context.Lotes.FindAsync(detalle.LoteId.Value);
                        if (lote != null)
                            lote.Cantidad += detalle.Cantidad * detalle.FactorUnidades;
                    }
                }

                // 3. Revertir citas pagadas por esta venta: vuelven a "Completada" (el
                // estado natural justo antes de cobrarse) y se desvinculan de la venta. Si
                // no se revierte esto, una cita queda "Pagada" para siempre aunque la venta
                // que la pagó se haya anulado.
                if (venta.VentaDetallesServicios != null)
                {
                    var citaIds = venta.VentaDetallesServicios
                        .Where(s => s.CitaId.HasValue)
                        .Select(s => s.CitaId!.Value)
                        .Distinct()
                        .ToList();

                    if (citaIds.Any())
                    {
                        var estadoCompletada = await _context.EstadosCita.FirstOrDefaultAsync(e => e.Nombre == "Completada");
                        var citas = await _context.Citas.Where(c => citaIds.Contains(c.Id)).ToListAsync();
                        foreach (var cita in citas)
                        {
                            cita.VentaId = null;
                            if (estadoCompletada != null) cita.EstadoId = estadoCompletada.Id;
                        }
                    }
                }

                // 4. Restar del turno. Debe restar exactamente lo mismo que se sumó al
                // crearla (solo productos, ver Crear): si acá se restara venta.Total
                // completo, se restaría de más por la parte de servicios que nunca se
                // sumó, dejando Turno.TotalVentas en negativo o desfasado.
                if (venta.Turno != null)
                {
                    decimal subtotalProdAnulada = venta.VentaDetalles?.Sum(d => d.Subtotal) ?? 0;
                    decimal ivaProdAnulada = Math.Round(subtotalProdAnulada * (venta.PorcentajeIva / 100m), 2);
                    venta.Turno.TotalVentas -= subtotalProdAnulada + ivaProdAnulada;
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
