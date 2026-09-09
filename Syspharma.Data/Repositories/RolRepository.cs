using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface IRolMaestroRepository
    {
        Task<List<RolMaestroDto>> ObtenerTodos();
        Task<RolMaestroDto?> ObtenerPorId(int id);
        Task<RolMaestroDto> Crear(RolMaestroDto dto);
        Task<RolMaestroDto> Actualizar(RolMaestroDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
        Task<List<string>> ObtenerPermisosPorRol(int rolId);
        Task<bool> AsignarPermisos(int rolId, List<string> permisos);
    }

    public class RolMaestroRepository : IRolMaestroRepository
    {
        private readonly SyspharmaContext _context;
        public RolMaestroRepository(SyspharmaContext context) => _context = context;

        public async Task<List<RolMaestroDto>> ObtenerTodos()
        {
            return await _context.Roles
                .AsNoTracking()
                .Select(r => new RolMaestroDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado,
                    FechaCreacion = r.FechaCreacion,
                    Permisos = r.RolesPermisos.Select(rp => rp.Permiso.Codigo).ToList()
                })
                .ToListAsync();
        }

        public async Task<RolMaestroDto?> ObtenerPorId(int id)
        {
            var rol = await _context.Roles
                .AsNoTracking()
                .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rol == null) return null;

            return new RolMaestroDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                Estado = rol.Estado,
                FechaCreacion = rol.FechaCreacion,
                Permisos = rol.RolesPermisos.Select(rp => rp.Permiso.Codigo).ToList()
            };
        }

        public async Task<RolMaestroDto> Crear(RolMaestroDto dto)
        {
            var rol = new Role
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado,
                FechaCreacion = DateTime.Now
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            if (dto.Permisos != null && dto.Permisos.Any())
            {
                await AsignarPermisos(rol.Id, dto.Permisos);
            }

            return await ObtenerPorId(rol.Id) ?? dto;
        }

        public async Task<RolMaestroDto> Actualizar(RolMaestroDto dto)
        {
            var rol = await _context.Roles.FindAsync(dto.Id);
            if (rol == null) throw new Exception("Rol no encontrado");

            rol.Nombre = dto.Nombre;
            rol.Descripcion = dto.Descripcion;
            rol.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            if (dto.Permisos != null)
            {
                await AsignarPermisos(rol.Id, dto.Permisos);
            }

            return await ObtenerPorId(rol.Id) ?? dto;
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return false;

            rol.Estado = estado;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Eliminar(int id)
        {
            var rol = await _context.Roles
                .Include(r => r.RolesPermisos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rol == null) return false;

            _context.RolesPermisos.RemoveRange(rol.RolesPermisos);
            _context.Roles.Remove(rol);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<string>> ObtenerPermisosPorRol(int rolId)
        {
            return await _context.RolesPermisos
                .Where(rp => rp.RoleId == rolId)
                .Select(rp => rp.Permiso.Codigo)
                .ToListAsync();
        }

        public async Task<bool> AsignarPermisos(int rolId, List<string> permisos)
        {
            var existentes = await _context.RolesPermisos
                .Where(rp => rp.RoleId == rolId)
                .ToListAsync();

            _context.RolesPermisos.RemoveRange(existentes);

            var permisosEntidad = await _context.Permisos
                .Where(p => permisos.Contains(p.Codigo))
                .ToListAsync();

            foreach (var permiso in permisosEntidad)
            {
                _context.RolesPermisos.Add(new RolesPermiso
                {
                    RoleId = rolId,
                    PermisoId = permiso.Id
                });
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}