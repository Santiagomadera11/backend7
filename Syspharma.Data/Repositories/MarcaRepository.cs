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
    public interface IMarcaRepository
    {
        Task<List<MarcaDto>> ObtenerTodos();
        Task<List<MarcaDto>> ObtenerTodosConInactivos(); // para la página de gestión de marcas
        Task<MarcaDto?> ObtenerPorId(int id);
        Task<MarcaDto> Crear(MarcaCreateDto dto);
        Task<MarcaDto> Actualizar(MarcaUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> CambiarEstado(int id, bool estado);
    }

    public class MarcaRepository : IMarcaRepository
    {
        private readonly SyspharmaContext _context;
        public MarcaRepository(SyspharmaContext context) => _context = context;

        private static MarcaDto ToDto(Marca m) => new MarcaDto
        {
            Id = m.Id,
            Nombre = m.Nombre,
            Descripcion = m.Descripcion,
            Estado = m.Estado,
            FechaCreacion = m.FechaCreacion
        };

        // Solo activas — para dropdowns de productos
        public async Task<List<MarcaDto>> ObtenerTodos()
        {
            var lista = await _context.Marcas
                .Where(m => m.Estado == true)
                .ToListAsync();
            return lista.Select(ToDto).ToList();
        }

        // Activas + inactivas — para la página de gestión de marcas.
        // El conteo de productos se calcula con un GROUP BY liviano en vez de
        // traer la tabla de productos completa (con todos sus joins) al cliente.
        public async Task<List<MarcaDto>> ObtenerTodosConInactivos()
        {
            var lista = await _context.Marcas.ToListAsync();
            var conteos = await _context.Productos
                .Where(p => p.MarcaId != null)
                .GroupBy(p => p.MarcaId)
                .Select(g => new { MarcaId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MarcaId!.Value, g => g.Count);

            return lista.Select(m =>
            {
                var dto = ToDto(m);
                dto.ProductosCount = conteos.TryGetValue(m.Id, out var count) ? count : 0;
                return dto;
            }).ToList();
        }

        public async Task<MarcaDto?> ObtenerPorId(int id)
        {
            var m = await _context.Marcas.FindAsync(id);
            return m == null ? null : ToDto(m);
        }

        public async Task<MarcaDto> Crear(MarcaCreateDto dto)
        {
            var m = new Marca { Nombre = dto.Nombre, Descripcion = dto.Descripcion, Estado = true, FechaCreacion = DateTime.Now };
            _context.Marcas.Add(m);
            await _context.SaveChangesAsync();
            return ToDto(m);
        }

        public async Task<MarcaDto> Actualizar(MarcaUpdateDto dto)
        {
            var m = await _context.Marcas.FindAsync(dto.Id);
            if (m == null) throw new Exception("Marca no encontrada");
            m.Nombre = dto.Nombre;
            m.Descripcion = dto.Descripcion;
            await _context.SaveChangesAsync();
            return ToDto(m);
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var m = await _context.Marcas.FindAsync(id);
            if (m == null) return false;
            m.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var m = await _context.Marcas.FindAsync(id);
            if (m == null) return false;

            var tieneProductos = await _context.Productos.AnyAsync(p => p.MarcaId == id);
            if (tieneProductos)
            {
                throw new Exception("No se puede eliminar la marca porque está relacionada a un producto.");
            }

            _context.Marcas.Remove(m);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
