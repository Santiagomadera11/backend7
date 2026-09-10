using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface ICarritoRepository
    {
        Task<List<CarritoItemDto>> ObtenerPorUsuario(int usuarioId);
        Task<List<CarritoItemDto>> UpsertItem(int usuarioId, CarritoItemUpsertDto dto);
        Task<List<CarritoItemDto>> EliminarItem(int usuarioId, int productoId, int? formaVentaId);
        Task VaciarCarrito(int usuarioId);
    }

    public class CarritoRepository : ICarritoRepository
    {
        private readonly SyspharmaContext _context;
        public CarritoRepository(SyspharmaContext context) => _context = context;

        private static CarritoItemDto ToDto(CarritoItem i)
        {
            var forma = i.FormaVentaId.HasValue
                ? i.Producto?.FormasVenta?.FirstOrDefault(f => f.Id == i.FormaVentaId.Value)
                : null;

            return new CarritoItemDto
            {
                Id = i.Id,
                ProductoId = i.ProductoId,
                Nombre = i.Producto?.Nombre ?? "",
                Precio = i.PrecioUnitario,
                Imagen = i.Producto?.Imagen,
                Cantidad = i.Cantidad,
                Stock = i.Producto?.Stock ?? 0,
                FormaVentaId = i.FormaVentaId,
                FormaVentaTipo = forma?.Tipo,
                FactorUnidades = forma?.FactorUnidades ?? 1,
            };
        }

        private async Task<Carrito> ObtenerOCrearCarrito(int usuarioId)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items).ThenInclude(i => i.Producto).ThenInclude(p => p.FormasVenta)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = usuarioId, FechaActualizacion = DateTime.Now };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return carrito;
        }

        public async Task<List<CarritoItemDto>> ObtenerPorUsuario(int usuarioId)
        {
            var carrito = await ObtenerOCrearCarrito(usuarioId);
            return carrito.Items.Select(ToDto).OrderBy(i => i.Id).ToList();
        }

        public async Task<List<CarritoItemDto>> UpsertItem(int usuarioId, CarritoItemUpsertDto dto)
        {
            var carrito = await ObtenerOCrearCarrito(usuarioId);

            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null) throw new Exception("Producto no encontrado");

            var cantidad = Math.Min(dto.Cantidad, Math.Max(producto.Stock, 0));
            if (cantidad <= 0) throw new Exception("El producto no tiene stock disponible");

            ProductoFormaVenta? formaVenta = null;
            if (dto.FormaVentaId.HasValue)
            {
                formaVenta = await _context.ProductoFormasVenta
                    .FirstOrDefaultAsync(f => f.Id == dto.FormaVentaId.Value && f.ProductoId == dto.ProductoId);
                if (formaVenta == null || !formaVenta.Activo)
                    throw new Exception("La forma de venta seleccionada no existe o fue deshabilitada.");
            }

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId && i.FormaVentaId == dto.FormaVentaId);
            if (item == null)
            {
                item = new CarritoItem { CarritoId = carrito.Id, ProductoId = dto.ProductoId, FormaVentaId = dto.FormaVentaId };
                _context.CarritoItems.Add(item);
            }
            item.Cantidad = cantidad;
            item.PrecioUnitario = formaVenta?.Precio ?? producto.Precio;

            carrito.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();

            return await ObtenerPorUsuario(usuarioId);
        }

        public async Task<List<CarritoItemDto>> EliminarItem(int usuarioId, int productoId, int? formaVentaId)
        {
            var carrito = await ObtenerOCrearCarrito(usuarioId);
            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId && i.FormaVentaId == formaVentaId);
            if (item != null)
            {
                _context.CarritoItems.Remove(item);
                carrito.FechaActualizacion = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return await ObtenerPorUsuario(usuarioId);
        }

        public async Task VaciarCarrito(int usuarioId)
        {
            var carrito = await ObtenerOCrearCarrito(usuarioId);
            _context.CarritoItems.RemoveRange(carrito.Items);
            carrito.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
