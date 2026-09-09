using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface IServicioRepository
    {
        Task<List<ServicioDto>> ObtenerTodos();
        Task<ServicioDto?> ObtenerPorId(int id);
        Task<ServicioDto> Crear(ServicioCreateDto dto);
        Task<ServicioDto> Actualizar(ServicioUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
    }

    public class ServicioRepository : IServicioRepository
    {
        private readonly SyspharmaContext _context;
        public ServicioRepository(SyspharmaContext context) => _context = context;

        public async Task<List<ServicioDto>> ObtenerTodos()
        {
            return await _context.Servicios
                .Include(s => s.Categoria)
                .Select(s => new ServicioDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    CategoriaId = s.CategoriaId,
                    CategoriaNombre = s.Categoria.Nombre,
                    Precio = s.Precio,
                    Duracion = s.Duracion,
                    Descripcion = s.Descripcion,
                    Estado = s.Estado,
                    FechaCreacion = s.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<ServicioDto?> ObtenerPorId(int id)
        {
            var s = await _context.Servicios
                .Include(s => s.Categoria)
                .FirstOrDefaultAsync(s => s.Id == id);

            return s == null ? null : new ServicioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                CategoriaId = s.CategoriaId,
                CategoriaNombre = s.Categoria.Nombre,
                Precio = s.Precio,
                Duracion = s.Duracion,
                Descripcion = s.Descripcion,
                Estado = s.Estado,
                FechaCreacion = s.FechaCreacion
            };
        }

        public async Task<ServicioDto> Crear(ServicioCreateDto dto)
        {
            var s = new Servicio
            {
                Nombre = dto.Nombre,
                CategoriaId = dto.CategoriaId,
                Precio = dto.Precio,
                Duracion = dto.Duracion,
                Descripcion = dto.Descripcion,
                Estado = true
            };
            _context.Servicios.Add(s);
            await _context.SaveChangesAsync();

            await _context.Entry(s).Reference(x => x.Categoria).LoadAsync();
            return new ServicioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                CategoriaId = s.CategoriaId,
                CategoriaNombre = s.Categoria?.Nombre,
                Precio = s.Precio,
                Duracion = s.Duracion,
                Descripcion = s.Descripcion,
                Estado = s.Estado,
                FechaCreacion = s.FechaCreacion
            };
        }

        public async Task<ServicioDto> Actualizar(ServicioUpdateDto dto)
        {
            var s = await _context.Servicios.FindAsync(dto.Id);
            if (s == null) throw new Exception("Servicio no encontrado");

            s.Nombre = dto.Nombre;
            s.CategoriaId = dto.CategoriaId;
            s.Precio = dto.Precio;
            s.Duracion = dto.Duracion;
            s.Descripcion = dto.Descripcion;
            s.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            await _context.Entry(s).Reference(x => x.Categoria).LoadAsync();
            return new ServicioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                CategoriaId = s.CategoriaId,
                CategoriaNombre = s.Categoria?.Nombre,
                Precio = s.Precio,
                Duracion = s.Duracion,
                Descripcion = s.Descripcion,
                Estado = s.Estado,
                FechaCreacion = s.FechaCreacion
            };
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var s = await _context.Servicios.FindAsync(id);
            if (s == null) return false;
            s.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var s = await _context.Servicios.FindAsync(id);
            if (s == null) return false;

            var enCitas = await _context.Citas.AnyAsync(c => c.ServicioId == id);
            var enVentas = await _context.VentaDetalleServicios.AnyAsync(v => v.ServicioId == id);

            if (enCitas || enVentas)
            {
                throw new Exception("No se puede eliminar el servicio porque está asociado a citas o ventas registradas. Desactívelo en su lugar.");
            }

            _context.Servicios.Remove(s);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}