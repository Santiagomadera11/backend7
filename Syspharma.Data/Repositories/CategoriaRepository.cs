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
    public interface ICategoriaRepository
    {
        Task<List<CategoriaDto>> ObtenerTodos();
        Task<List<CategoriaDto>> ObtenerTodosConInactivos(); // para la página de gestión de categorías
        Task<CategoriaDto?> ObtenerPorId(int id);
        Task<CategoriaDto> Crear(CategoriaCreateDto dto);
        Task<CategoriaDto> Actualizar(CategoriaUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> CambiarEstado(int id, bool estado);
    }

    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly SyspharmaContext _context;
        public CategoriaRepository(SyspharmaContext context) => _context = context;

        private static CategoriaDto ToDto(Categoria c) => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Estado = c.Estado
        };

        // BUG 2 FIX: solo devuelve activas — para dropdowns de productos, compras, etc.
        public async Task<List<CategoriaDto>> ObtenerTodos()
        {
            var lista = await _context.Categorias
                .Where(c => c.Estado == true)
                .ToListAsync();
            return lista.Select(ToDto).ToList();
        }

        // Para la página de gestión de categorías donde sí se necesitan ver todas
        public async Task<List<CategoriaDto>> ObtenerTodosConInactivos()
        {
            var lista = await _context.Categorias.ToListAsync();
            return lista.Select(ToDto).ToList();
        }

        public async Task<CategoriaDto?> ObtenerPorId(int id)
        {
            var c = await _context.Categorias.FindAsync(id);
            return c == null ? null : ToDto(c);
        }

        public async Task<CategoriaDto> Crear(CategoriaCreateDto dto)
        {
            var c = new Categoria { Nombre = dto.Nombre, Descripcion = dto.Descripcion, Estado = true };
            _context.Categorias.Add(c);
            await _context.SaveChangesAsync();
            return ToDto(c);
        }

        public async Task<CategoriaDto> Actualizar(CategoriaUpdateDto dto)
        {
            var c = await _context.Categorias.FindAsync(dto.Id);
            if (c == null) throw new Exception("Categoría no encontrada");
            c.Nombre = dto.Nombre;
            c.Descripcion = dto.Descripcion;
            await _context.SaveChangesAsync();
            return ToDto(c);
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return false;
            c.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return false;

            // --- NUEVO: Validar si la categoría está en uso por algún producto ---
            var tieneProductos = await _context.Productos.AnyAsync(p => p.CategoriaId == id);
            if (tieneProductos)
            {
                throw new Exception("No se puede eliminar la categoría porque está relacionada a un producto.");
            }

            // --- NUEVO: Validar si la categoría está en uso por algún servicio ---
            var tieneServicios = await _context.Servicios.AnyAsync(s => s.CategoriaId == id);
            if (tieneServicios)
            {
                throw new Exception("No se puede eliminar la categoría porque está relacionada a un servicio.");
            }

            _context.Categorias.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}