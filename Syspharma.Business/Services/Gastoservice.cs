using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IGastoService
    {
        Task<List<GastoDto>> ObtenerTodos();
        Task<List<GastoDto>> ObtenerPorTurno(int turnoId);
        Task<List<GastoDto>> ObtenerHoy(int? usuarioId);
        Task<GastoDto?> ObtenerPorId(int id);
        Task<GastoDto> Crear(GastoCreateDto dto);
        Task<GastoDto> Actualizar(GastoUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> Anular(int id, string? motivo);
    }

    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _repo;
        public GastoService(IGastoRepository repo) => _repo = repo;

        public Task<List<GastoDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<List<GastoDto>> ObtenerPorTurno(int turnoId) => _repo.ObtenerPorTurno(turnoId);
        public Task<List<GastoDto>> ObtenerHoy(int? usuarioId) => _repo.ObtenerHoy(usuarioId);
        public Task<GastoDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<GastoDto> Crear(GastoCreateDto dto) => _repo.Crear(dto);
        public Task<GastoDto> Actualizar(GastoUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<bool> Anular(int id, string? motivo) => _repo.Anular(id, motivo);
    }
}
