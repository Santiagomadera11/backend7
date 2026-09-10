using System.Collections.Generic;
using System.Threading.Tasks;
using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IMarcaService
    {
        Task<List<MarcaDto>> ObtenerTodos();
        Task<List<MarcaDto>> ObtenerTodosConInactivos();
        Task<MarcaDto?> ObtenerPorId(int id);
        Task<MarcaDto> Crear(MarcaCreateDto dto);
        Task<MarcaDto> Actualizar(MarcaUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> CambiarEstado(int id, bool estado);
    }

    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _repo;
        public MarcaService(IMarcaRepository repo) => _repo = repo;

        public Task<List<MarcaDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<List<MarcaDto>> ObtenerTodosConInactivos() => _repo.ObtenerTodosConInactivos();
        public Task<MarcaDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<MarcaDto> Crear(MarcaCreateDto dto) => _repo.Crear(dto);
        public Task<MarcaDto> Actualizar(MarcaUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<bool> CambiarEstado(int id, bool estado) => _repo.CambiarEstado(id, estado);
    }
}
