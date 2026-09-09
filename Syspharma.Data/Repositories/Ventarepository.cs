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
    public interface IVentaRepository
    {
        Task<List<VentaDto>> ObtenerTodos();
        Task<VentaDto?> ObtenerPorId(int id);
        Task<VentaDto> Crear(VentaCreateDto dto);
        Task<VentaDto> Actualizar(VentaUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Anular(int id);
        Task<bool> Eliminar(int id);
        Task<List<object>> ObtenerEstados();
    }

    public class VentaRepository : IVentaRepository
    {
        private readonly SyspharmaContext _context;

        public VentaRepository(SyspharmaContext context)
        {
            _context = context;
        }

        private static VentaDto MapDto(Venta v) => new VentaDto
        {
            Id = v.Id,
            NumeroVenta = v.NumeroVenta,
            TurnoId = v.TurnoId,
            UsuarioId = v.UsuarioId,
            UsuarioNombre = v.Usuario?.Nombre ?? "N/A",
            ClienteNombre = v.ClienteNombre,
            MetodoPagoNombre = v.MetodoPago?.Nombre ?? "Efectivo",
            EstadoId = v.EstadoId,
            EstadoNombre = v.Estado?.Nombre ?? "Completada",
            Subtotal = v.Subtotal,
            Iva = v.Iva,
            Total = v.Total,
            FechaVenta = v.FechaVenta,
            Detalles = v.VentaDetalles?.Select(d => new VentaDetalleDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                ProductoNombre = d.Producto?.Nombre ?? "Producto",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal,
                LoteId = d.LoteId
            }).ToList() ?? new List<VentaDetalleDto>(),
            Servicios = v.VentaDetallesServicios?.Select(s => new VentaDetalleServicioDto
            {
                Id = s.Id,
                ServicioId = s.ServicioId,
                ServicioNombre = s.Servicio?.Nombre ?? "Servicio",
                Cantidad = s.Cantidad,
                PrecioUnitario = s.PrecioUnitario,
                Subtotal = s.Subtotal
            }).ToList() ?? new List<VentaDetalleServicioDto>()
        };

        public async Task<List<VentaDto>> ObtenerTodos()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.MetodoPago)
                .Include(v => v.Estado)
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Producto)
                .Include(v => v.VentaDetallesServicios).ThenInclude(s => s.Servicio)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
            return ventas.Select(MapDto).ToList();
        }

        public async Task<VentaDto?> ObtenerPorId(int id)
        {
            var v = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.MetodoPago)
                .Include(v => v.Estado)
                .Include(v => v.VentaDetalles).ThenInclude(d => d.Producto)
                .Include(v => v.VentaDetallesServicios).ThenInclude(s => s.Servicio)
                .FirstOrDefaultAsync(v => v.Id == id);
            return v == null ? null : MapDto(v);
        }

        public async Task<VentaDto> Crear(VentaCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal subtotalProductos = dto.Detalles?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
                decimal subtotalServicios = dto.Servicios?.Sum(s => s.Cantidad * s.PrecioUnitario) ?? 0;
                decimal subtotalFinal = subtotalProductos + subtotalServicios;
                decimal iva = Math.Round(subtotalFinal * (dto.PorcentajeIva / 100), 2);
                decimal totalFinal = subtotalFinal + iva;

                var venta = new Venta
                {
                    NumeroVenta = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TurnoId = dto.TurnoId,
                    UsuarioId = dto.UsuarioId,
                    ClienteNombre = dto.ClienteNombre ?? "Consumidor Final",
                    MetodoPagoId = dto.MetodoPagoId,
                    EstadoId = 1,
                    Subtotal = subtotalFinal,
                    Iva = iva,
                    Total = totalFinal,
                    Notas = dto.Notas,
                    FechaVenta = DateTime.Now
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                if (dto.Detalles != null)
                {
                    foreach (var d in dto.Detalles)
                    {
                        int? assignedLoteId = d.LoteId;

                        if (assignedLoteId.HasValue)
                        {
                            var lote = await _context.Lotes.FindAsync(assignedLoteId.Value);
                            if (lote != null)
                            {
                                lote.Cantidad -= d.Cantidad;
                            }
                        }
                        else
                        {
                            // Fallback FEFO automático si el producto tiene lotes activos
                            var activeLotes = await _context.Lotes
                                .Where(l => l.ProductoId == d.ProductoId && l.FechaVencimiento >= DateOnly.FromDateTime(DateTime.Today) && l.Cantidad > 0)
                                .OrderBy(l => l.FechaVencimiento)
                                .ToListAsync();

                            if (activeLotes.Any())
                            {
                                int pendingToDiscount = d.Cantidad;
                                foreach (var lote in activeLotes)
                                {
                                    if (pendingToDiscount <= 0) break;
                                    int toDiscount = Math.Min(lote.Cantidad, pendingToDiscount);
                                    lote.Cantidad -= toDiscount;
                                    pendingToDiscount -= toDiscount;
                                    if (!assignedLoteId.HasValue)
                                    {
                                        assignedLoteId = lote.Id; // Asociar al menos el primer lote modificado
                                    }
                                }
                            }
                        }

                        _context.VentaDetalles.Add(new VentaDetalle
                        {
                            VentaId = venta.Id,
                            ProductoId = d.ProductoId,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Subtotal = d.Cantidad * d.PrecioUnitario,
                            LoteId = assignedLoteId
                        });

                        var p = await _context.Productos.FindAsync(d.ProductoId);
                        if (p != null) p.Stock -= d.Cantidad;
                    }
                }

                if (dto.Servicios != null)
                {
                    foreach (var s in dto.Servicios)
                    {
                        _context.VentaDetalleServicios.Add(new VentaDetalleServicio
                        {
                            VentaId = venta.Id,
                            ServicioId = s.ServicioId,
                            Cantidad = s.Cantidad,
                            PrecioUnitario = s.PrecioUnitario,
                            Subtotal = s.Cantidad * s.PrecioUnitario,
                            CitaId = s.CitaId
                        });

                        // Mark Cita as Pagada
                        if (s.CitaId.HasValue && s.CitaId.Value > 0)
                        {
                            var cita = await _context.Citas.FindAsync(s.CitaId.Value);
                            if (cita != null)
                            {
                                cita.VentaId = venta.Id;
                                var estadoPagada = await _context.EstadosCita.FirstOrDefaultAsync(e => e.Nombre == "Pagada");
                                if (estadoPagada != null)
                                {
                                    cita.EstadoId = estadoPagada.Id;
                                }
                            }
                        }
                    }
                }

                var turno = await _context.Turnos.FindAsync(dto.TurnoId);
                if (turno != null)
                {
                    turno.TotalVentas += totalFinal;
                    turno.ResumenVentas += 1;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await ObtenerPorId(venta.Id) ?? MapDto(venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<VentaDto> Actualizar(VentaUpdateDto dto)
        {
            var venta = await _context.Ventas.FindAsync(dto.Id)
                ?? throw new Exception("Venta no encontrada");

            venta.ClienteNombre = dto.ClienteNombre;
            venta.MetodoPagoId = dto.MetodoPagoId;
            venta.EstadoId = dto.EstadoId;
            venta.Notas = dto.Notas;

            await _context.SaveChangesAsync();
            return await ObtenerPorId(venta.Id) ?? MapDto(venta);
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.VentaDetalles)
                    .FirstOrDefaultAsync(v => v.Id == id);
                if (venta == null) return false;

                // Revertir stock
                foreach (var d in venta.VentaDetalles)
                {
                    if (d.LoteId.HasValue)
                    {
                        var lote = await _context.Lotes.FindAsync(d.LoteId.Value);
                        if (lote != null)
                        {
                            lote.Cantidad += d.Cantidad;
                        }
                    }
                    var p = await _context.Productos.FindAsync(d.ProductoId);
                    if (p != null) p.Stock += d.Cantidad;
                }

                // Revertir turno
                var turno = await _context.Turnos.FindAsync(venta.TurnoId);
                if (turno != null)
                {
                    turno.TotalVentas -= venta.Total;
                    turno.ResumenVentas -= 1;
                }

                venta.EstadoId = 3; // anulada
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.VentaDetalles)
                .Include(v => v.VentaDetallesServicios)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (venta == null) return false;

            _context.VentaDetalles.RemoveRange(venta.VentaDetalles);
            _context.VentaDetalleServicios.RemoveRange(venta.VentaDetallesServicios);
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<object>> ObtenerEstados() =>
            await _context.EstadosVenta.Select(e => (object)new { e.Id, e.Nombre }).ToListAsync();
    }
}
