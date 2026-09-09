using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface ICitaService
    {
        Task<List<CitaDto>> ObtenerTodos();
        Task<CitaDto?> ObtenerPorId(int id);
        Task<CitaDto> Crear(CitaCreateDto dto);
        Task<CitaDto> Actualizar(CitaUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Eliminar(int id);
        Task<List<CitaEstadoDto>> ObtenerEstados(); 
    }

    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _repo;
        public CitaService(ICitaRepository repo) => _repo = repo;
        public Task<List<CitaDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<CitaDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<CitaDto> Crear(CitaCreateDto dto) => _repo.Crear(dto);
        public Task<CitaDto> Actualizar(CitaUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> CambiarEstado(int id, int estadoId) => _repo.CambiarEstado(id, estadoId);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<List<CitaEstadoDto>> ObtenerEstados() => _repo.ObtenerEstados();
    }
}