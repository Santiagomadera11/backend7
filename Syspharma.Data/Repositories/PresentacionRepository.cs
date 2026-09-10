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
    public interface IPresentacionRepository
    {
        Task<List<PresentacionDto>> ObtenerTodos();
        Task<List<PresentacionDto>> ObtenerTodosConInactivos(); // para la página de gestión de presentaciones
        Task<PresentacionDto?> ObtenerPorId(int id);
        Task<PresentacionDto> Crear(PresentacionCreateDto dto);
        Task<PresentacionDto> Actualizar(PresentacionUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> CambiarEstado(int id, bool estado);
    }

    public class PresentacionRepository : IPresentacionRepository
    {
        private readonly SyspharmaContext _context;
        public PresentacionRepository(SyspharmaContext context) => _context = context;

        private static PresentacionDto ToDto(Presentacion p) => new PresentacionDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Estado = p.Estado,
            FechaCreacion = p.FechaCreacion
        };

        // Solo activas — para dropdowns de productos
        public async Task<List<PresentacionDto>> ObtenerTodos()
        {
            var lista = await _context.Presentaciones
                .Where(p => p.Estado == true)
                .ToListAsync();
            return lista.Select(ToDto).ToList();
        }

        // Activas + inactivas — para la página de gestión de presentaciones.
        // El conteo de productos se calcula con un GROUP BY liviano en vez de
        // traer la tabla de productos completa (con todos sus joins) al cliente.
        public async Task<List<PresentacionDto>> ObtenerTodosConInactivos()
        {
            var lista = await _context.Presentaciones.ToListAsync();
            var conteos = await _context.Productos
                .Where(p => p.PresentacionId != null)
                .GroupBy(p => p.PresentacionId)
                .Select(g => new { PresentacionId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.PresentacionId!.Value, g => g.Count);

            return lista.Select(p =>
            {
                var dto = ToDto(p);
                dto.ProductosCount = conteos.TryGetValue(p.Id, out var count) ? count : 0;
                return dto;
            }).ToList();
        }

        public async Task<PresentacionDto?> ObtenerPorId(int id)
        {
            var p = await _context.Presentaciones.FindAsync(id);
            return p == null ? null : ToDto(p);
        }

        public async Task<PresentacionDto> Crear(PresentacionCreateDto dto)
        {
            var p = new Presentacion { Nombre = dto.Nombre, Descripcion = dto.Descripcion, Estado = true, FechaCreacion = DateTime.Now };
            _context.Presentaciones.Add(p);
            await _context.SaveChangesAsync();
            return ToDto(p);
        }

        public async Task<PresentacionDto> Actualizar(PresentacionUpdateDto dto)
        {
            var p = await _context.Presentaciones.FindAsync(dto.Id);
            if (p == null) throw new Exception("Presentación no encontrada");
            p.Nombre = dto.Nombre;
            p.Descripcion = dto.Descripcion;
            await _context.SaveChangesAsync();
            return ToDto(p);
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var p = await _context.Presentaciones.FindAsync(id);
            if (p == null) return false;
            p.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var p = await _context.Presentaciones.FindAsync(id);
            if (p == null) return false;

            var tieneProductos = await _context.Productos.AnyAsync(prod => prod.PresentacionId == id);
            if (tieneProductos)
            {
                throw new Exception("No se puede eliminar la presentación porque está relacionada a un producto.");
            }

            _context.Presentaciones.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
