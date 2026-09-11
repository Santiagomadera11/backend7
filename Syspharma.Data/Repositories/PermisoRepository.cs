using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface IPermisoRepository
    {
        Task<List<PermisoDto>> ObtenerTodos();
        Task<PermisoDto?> ObtenerPorId(int id);
        Task<PermisoDto> Crear(PermisoCreateDto dto);
        Task<PermisoDto> Actualizar(PermisoUpdateDto dto);
        Task<bool> Eliminar(int id);
    }

    public class PermisoRepository : IPermisoRepository
    {
        private readonly SyspharmaContext _context;
        public PermisoRepository(SyspharmaContext context) => _context = context;

        public async Task<List<PermisoDto>> ObtenerTodos() => (await _context.Permisos.ToListAsync()).Select(p => new PermisoDto { Id = p.Id, Codigo = p.Codigo, Nombre = p.Nombre, Categoria = p.Categoria }).ToList();
        public async Task<PermisoDto?> ObtenerPorId(int id) { var p = await _context.Permisos.FindAsync(id); return p == null ? null : new PermisoDto { Id = p.Id, Codigo = p.Codigo }; }
        public Task<PermisoDto> Crear(PermisoCreateDto dto) => throw new NotImplementedException();
        public Task<PermisoDto> Actualizar(PermisoUpdateDto dto) => throw new NotImplementedException();
        public Task<bool> Eliminar(int id) => throw new NotImplementedException();
    }
}