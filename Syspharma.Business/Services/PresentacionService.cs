using System.Collections.Generic;
using System.Threading.Tasks;
using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IPresentacionService
    {
        Task<List<PresentacionDto>> ObtenerTodos();
        Task<List<PresentacionDto>> ObtenerTodosConInactivos();
        Task<PresentacionDto?> ObtenerPorId(int id);
        Task<PresentacionDto> Crear(PresentacionCreateDto dto);
        Task<PresentacionDto> Actualizar(PresentacionUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> CambiarEstado(int id, bool estado);
    }

    public class PresentacionService : IPresentacionService
    {
        private readonly IPresentacionRepository _repo;
        public PresentacionService(IPresentacionRepository repo) => _repo = repo;

        public Task<List<PresentacionDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<List<PresentacionDto>> ObtenerTodosConInactivos() => _repo.ObtenerTodosConInactivos();
        public Task<PresentacionDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<PresentacionDto> Crear(PresentacionCreateDto dto) => _repo.Crear(dto);
        public Task<PresentacionDto> Actualizar(PresentacionUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<bool> CambiarEstado(int id, bool estado) => _repo.CambiarEstado(id, estado);
    }
}
