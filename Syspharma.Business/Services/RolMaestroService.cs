using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IRolMaestroService
    {
        Task<List<RolMaestroDto>> ObtenerTodos();
        Task<RolMaestroDto?> ObtenerPorId(int id);
        Task<RolMaestroDto> Crear(RolMaestroDto dto);
        Task<RolMaestroDto> Actualizar(RolMaestroDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
        Task<List<string>> ObtenerPermisos(int rolId);
        Task<bool> AsignarPermisos(int rolId, List<string> permisos);
    }

    public class RolMaestroService : IRolMaestroService
    {
        private readonly IRolMaestroRepository _repo;
        public RolMaestroService(IRolMaestroRepository repo) => _repo = repo;

        public Task<List<RolMaestroDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<RolMaestroDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<RolMaestroDto> Crear(RolMaestroDto dto) => _repo.Crear(dto);
        public Task<RolMaestroDto> Actualizar(RolMaestroDto dto) => _repo.Actualizar(dto);
        public Task<bool> CambiarEstado(int id, bool estado) => _repo.CambiarEstado(id, estado);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<List<string>> ObtenerPermisos(int rolId) => _repo.ObtenerPermisosPorRol(rolId);
        public Task<bool> AsignarPermisos(int rolId, List<string> permisos) => _repo.AsignarPermisos(rolId, permisos);
    }
}