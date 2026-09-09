using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IProveedorService
    {
        Task<List<ProveedorDto>> ObtenerTodos();
        Task<ProveedorDto?> ObtenerPorId(int id);
        Task<ProveedorDto> Crear(ProveedorCreateDto dto);
        Task<ProveedorDto> Actualizar(ProveedorUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
    }

    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _repo;
        public ProveedorService(IProveedorRepository repo) => _repo = repo;
        public Task<List<ProveedorDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<ProveedorDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<ProveedorDto> Crear(ProveedorCreateDto dto) => _repo.Crear(dto);
        public Task<ProveedorDto> Actualizar(ProveedorUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> CambiarEstado(int id, bool estado) => _repo.CambiarEstado(id, estado);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
    }
}