using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface ICompraService
    {
        Task<List<CompraDto>> ObtenerTodos();
        Task<CompraDto?> ObtenerPorId(int id);
        Task<CompraDto> Crear(CompraCreateDto dto);
        Task<CompraDto> Actualizar(CompraUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Eliminar(int id);
        Task<List<object>> ObtenerEstados();
    }

    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _repo;
        public CompraService(ICompraRepository repo) => _repo = repo;

        public Task<List<CompraDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<CompraDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<CompraDto> Crear(CompraCreateDto dto) => _repo.Crear(dto);
        public Task<CompraDto> Actualizar(CompraUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> CambiarEstado(int id, int estadoId) => _repo.CambiarEstado(id, estadoId);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<List<object>> ObtenerEstados() => _repo.ObtenerEstados();
    }
}