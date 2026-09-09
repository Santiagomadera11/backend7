using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    public interface ITurnoService
    {
        Task<List<TurnoDto>> ObtenerTodos();
        Task<TurnoDto?> ObtenerPorId(int id);
        Task<TurnoDto?> ObtenerTurnoActivo(int usuarioId);
        Task<TurnoDto> Abrir(TurnoAbrirDto dto);
        Task<TurnoDto> Cerrar(TurnoCerrarDto dto);
        Task<bool> Eliminar(int id);
    }

    public class TurnoService : ITurnoService
    {
        private readonly ITurnoRepository _repo;
        public TurnoService(ITurnoRepository repo) => _repo = repo;

        public Task<List<TurnoDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<TurnoDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<TurnoDto?> ObtenerTurnoActivo(int usuarioId) => _repo.ObtenerTurnoActivo(usuarioId);
        public Task<TurnoDto> Abrir(TurnoAbrirDto dto) => _repo.Abrir(dto);
        public Task<TurnoDto> Cerrar(TurnoCerrarDto dto) => _repo.Cerrar(dto);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
    }
}