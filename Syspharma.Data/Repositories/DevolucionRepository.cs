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
        Task<List<MermaDto>> ObtenerMermas(DateTime? desde, DateTime? hasta);
    }

    public class DevolucionRepository : IDevolucionRepository
    {
        private readonly SyspharmaContext _context;
        public DevolucionRepository(SyspharmaContext context) => _context = context;

        private static DevolucionDto MapDto(Devolucion d) => new DevolucionDto
        {
            Id = d.Id,
            VentaId = d.VentaId,
            NumeroVenta = d.Venta?.NumeroVenta ?? "",
            UsuarioId = d.UsuarioId,
            UsuarioNombre = d.Usuario?.Nombre ?? "",
            EstadoId = d.EstadoId,
            EstadoNombre = d.Estado?.Nombre ?? "",
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
                ProductoNombre = det.Producto?.Nombre ?? "",
                CantidadDevuelta = det.CantidadDevuelta,
                PrecioUnitario = det.PrecioUnitario,
                SubtotalDevuelto = det.SubtotalDevuelto ?? 0,
                Reingresa = det.Reingresa
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
                // Una venta anulada ya devolvió el 100% de su stock/lotes en VentaService.Anular.
                // Si se permitiera además una devolución sobre ella, al aprobarla se acreditaría
                // ese stock una segunda vez.
                var ventaOrigen = await _context.Ventas.FindAsync(dto.VentaId)
                    ?? throw new Exception("La venta no existe.");
                if (ventaOrigen.EstadoId == 3)
                    throw new Exception("No se puede registrar una devolución sobre una venta anulada: su stock ya fue restaurado por completo al anularla.");

                // Obtener precios de los detalles de venta
                var detalleVentaIds = dto.Detalles.Select(d => d.DetalleVentaId).ToList();
                var detallesVenta = await _context.VentaDetalles
                    .Where(d => detalleVentaIds.Contains(d.Id))
                    .ToListAsync();

                // Cuánto se ha devuelto ya de cada línea (contando Pendientes y Aprobadas,
                // pero no Rechazadas), para no permitir devolver más de lo que la venta
                // realmente entregó ni duplicar una devolución ya registrada.
                var yaDevueltoPorLinea = await _context.DetallesDevoluciones
                    .Where(dd => detalleVentaIds.Contains(dd.DetalleVentaId) && dd.Devolucion.EstadoId != 3)
                    .GroupBy(dd => dd.DetalleVentaId)
                    .Select(g => new { DetalleVentaId = g.Key, Cantidad = g.Sum(x => x.CantidadDevuelta) })
                    .ToDictionaryAsync(g => g.DetalleVentaId, g => g.Cantidad);

                var detalles = new List<DetalleDevolucion>();
                // Acumula lo que esta MISMA solicitud ya reservó por línea, para que dos
                // entradas con el mismo DetalleVentaId en un solo request no se validen
                // cada una por separado contra el mismo disponible (lo que permitiría que
                // la suma de ambas exceda lo realmente vendido).
                var acumuladoEnEstaSolicitud = new Dictionary<int, int>();
                foreach (var d in dto.Detalles)
                {
                    var dventa = detallesVenta.FirstOrDefault(dv => dv.Id == d.DetalleVentaId)
                        ?? throw new Exception($"El detalle de venta {d.DetalleVentaId} no existe.");

                    if (dventa.ProductoId != d.ProductoId)
                        throw new Exception($"El producto indicado no coincide con el vendido en el detalle {d.DetalleVentaId}.");

                    var yaDevuelto = yaDevueltoPorLinea.GetValueOrDefault(d.DetalleVentaId, 0);
                    var acumuladoPrevio = acumuladoEnEstaSolicitud.GetValueOrDefault(d.DetalleVentaId, 0);
                    var disponible = dventa.Cantidad - yaDevuelto - acumuladoPrevio;
                    if (d.CantidadDevuelta > disponible)
                        throw new Exception($"No puedes devolver {d.CantidadDevuelta} unidades del producto {d.ProductoId}: ya se vendieron {dventa.Cantidad}, {yaDevuelto} ya están en otra devolución (pendiente o aprobada) y {acumuladoPrevio} ya se reservaron en esta misma solicitud. Disponible para devolver: {Math.Max(disponible, 0)}.");

                    acumuladoEnEstaSolicitud[d.DetalleVentaId] = acumuladoPrevio + d.CantidadDevuelta;

                    var precio = dventa.PrecioUnitario;
                    detalles.Add(new DetalleDevolucion
                    {
                        DetalleVentaId = d.DetalleVentaId,
                        ProductoId = d.ProductoId,
                        CantidadDevuelta = d.CantidadDevuelta,
                        PrecioUnitario = precio,
                        SubtotalDevuelto = d.CantidadDevuelta * precio,
                        Reingresa = d.Reingresa
                    });
                }

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

                // Ni el stock ni el estado de la venta cambian acá: la devolución nace
                // "Pendiente" (EstadoId=1) y solo debe impactar el inventario y marcar la
                // venta como "Devolución" cuando un supervisor la aprueba (ver Gestionar).
                // Así se evita que cualquiera infle el stock o bloquee la anulación de una
                // venta con solo registrar una devolución, sin que nadie la revise.

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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var devolucion = await _context.Devoluciones
                    .Include(d => d.Detalles)
                    .FirstOrDefaultAsync(d => d.Id == id)
                    ?? throw new Exception("Devolución no encontrada");

                if (devolucion.EstadoId != 1)
                    throw new Exception("Esta devolución ya fue gestionada y no puede volver a procesarse.");

                // Si en el tiempo que la devolución estuvo pendiente la venta fue anulada,
                // VentaService.Anular ya restauró el 100% de su stock y lotes. Aprobar esta
                // devolución ahora volvería a sumar ese mismo stock una segunda vez.
                var venta = await _context.Ventas.FindAsync(devolucion.VentaId);
                if (dto.NuevoEstado == 2 && venta != null && venta.EstadoId == 3)
                    throw new Exception("No se puede aprobar esta devolución: la venta ya fue anulada y su stock se restauró por completo en ese momento. Rechaza la devolución en su lugar.");

                devolucion.EstadoId = dto.NuevoEstado;
                devolucion.UsuarioGestionId = dto.UsuarioGestionId;
                devolucion.FechaGestion = DateTime.Now;

                // El stock recién se reingresa acá, al aprobar (2). Si se rechaza (3),
                // el producto nunca vuelve al inventario, y la venta no cambia de estado.
                if (dto.NuevoEstado == 2)
                {
                    // El reembolso sale de la caja ACTUALMENTE ABIERTA de quien aprueba la
                    // devolución, no de la caja de la venta original: esa puede haberse
                    // cerrado hace mucho (su cuadre ya quedó registrado, igual que en
                    // Anular) y la plata físicamente sale del cajón que está abierto ahora.
                    // Todas las líneas de una devolución son de productos (nunca de
                    // servicios), así que TotalDevolucion siempre es "solo productos" —
                    // mismo criterio que ya usa Turno.TotalVentas al crear una venta.
                    var turnoAprobador = await _context.Turnos
                        .FirstOrDefaultAsync(t => t.UsuarioId == dto.UsuarioGestionId && t.Estado == "activo");
                    if (turnoAprobador != null)
                        turnoAprobador.TotalVentas -= devolucion.TotalDevolucion;

                    foreach (var det in devolucion.Detalles)
                    {
                        // Dañado/vencido: se pierde, no reingresa al stock vendible ni a ningún lote.
                        if (!det.Reingresa) continue;

                        var producto = await _context.Productos.FindAsync(det.ProductoId);
                        if (producto != null)
                        {
                            producto.Stock += det.CantidadDevuelta;
                            producto.UltimaActualizacion = DateTime.Now;
                        }

                        // Devolver la cantidad exacta a el/los mismos lotes de donde salió en
                        // la venta original (VentaDetalleLote), respetando lo que cada lote
                        // ya recibió por devoluciones aprobadas anteriores de esa misma línea.
                        var ventaDetalle = await _context.VentaDetalles
                            .Include(vd => vd.Lotes)
                            .FirstOrDefaultAsync(vd => vd.Id == det.DetalleVentaId);

                        if (ventaDetalle == null || !ventaDetalle.Lotes.Any()) continue;

                        var pendiente = det.CantidadDevuelta;
                        foreach (var ventaDetalleLote in ventaDetalle.Lotes)
                        {
                            if (pendiente <= 0) break;

                            var yaRestaurado = await _context.DetalleDevolucionLotes
                                .Where(ddl => ddl.LoteId == ventaDetalleLote.LoteId
                                              && ddl.DetalleDevolucion.DetalleVentaId == det.DetalleVentaId
                                              && ddl.DetalleDevolucion.Devolucion.EstadoId == 2)
                                .SumAsync(ddl => (int?)ddl.Cantidad) ?? 0;

                            var capacidadRestante = ventaDetalleLote.Cantidad - yaRestaurado;
                            if (capacidadRestante <= 0) continue;

                            var aRestaurar = Math.Min(capacidadRestante, pendiente);
                            var lote = await _context.Lotes.FindAsync(ventaDetalleLote.LoteId);
                            if (lote != null) lote.Cantidad += aRestaurar;

                            det.Lotes.Add(new DetalleDevolucionLote { LoteId = ventaDetalleLote.LoteId, Cantidad = aRestaurar });
                            pendiente -= aRestaurar;
                        }
                    }

                    // Recién ahora, al aprobar, la venta pasa a estado "Devolución". El caso
                    // "ya anulada" ya se descartó arriba con una excepción, así que acá
                    // siempre es seguro marcarla.
                    if (venta != null) venta.EstadoId = 2;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await ObtenerPorId(id) ?? MapDto(devolucion);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<EstadoDevolucionDto>> ObtenerEstados() =>
            await _context.EstadosDevoluciones
                .Where(e => e.Activo)
                .Select(e => new EstadoDevolucionDto { Id = e.Id, Nombre = e.Nombre, Activo = e.Activo })
                .ToListAsync();

        public async Task<List<MermaDto>> ObtenerMermas(DateTime? desde, DateTime? hasta)
        {
            // Solo cuentan como merma real las líneas de devoluciones ya APROBADAS (2)
            // marcadas como "no reingresa" (dañado/vencido). Una pendiente o rechazada
            // todavía no representa una pérdida confirmada.
            var query = _context.DetallesDevoluciones
                .Include(dd => dd.Producto)
                .Include(dd => dd.Devolucion).ThenInclude(d => d.Venta)
                .Where(dd => !dd.Reingresa && dd.Devolucion.EstadoId == 2);

            if (desde.HasValue) query = query.Where(dd => dd.Devolucion.FechaGestion >= desde.Value);
            if (hasta.HasValue) query = query.Where(dd => dd.Devolucion.FechaGestion <= hasta.Value);

            var detalles = await query
                .OrderByDescending(dd => dd.Devolucion.FechaGestion)
                .ToListAsync();

            var usuarioIds = detalles
                .Select(dd => dd.Devolucion.UsuarioGestionId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();
            var usuarios = await _context.Usuarios
                .Where(u => usuarioIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Nombre);

            return detalles.Select(dd => new MermaDto
            {
                DetalleDevolucionId = dd.Id,
                DevolucionId = dd.DevolucionId,
                VentaId = dd.Devolucion.VentaId,
                NumeroVenta = dd.Devolucion.Venta?.NumeroVenta ?? "",
                ProductoId = dd.ProductoId,
                ProductoNombre = dd.Producto?.Nombre ?? "",
                CantidadPerdida = dd.CantidadDevuelta,
                PrecioUnitario = dd.PrecioUnitario,
                ValorPerdida = dd.CantidadDevuelta * dd.PrecioUnitario,
                Motivo = dd.Devolucion.Motivo,
                Observaciones = dd.Devolucion.Observaciones,
                FechaGestion = dd.Devolucion.FechaGestion ?? dd.Devolucion.FechaDevolucion,
                UsuarioGestionNombre = dd.Devolucion.UsuarioGestionId.HasValue
                    && usuarios.TryGetValue(dd.Devolucion.UsuarioGestionId.Value, out var nombre)
                    ? nombre
                    : ""
            }).ToList();
        }
    }
}
