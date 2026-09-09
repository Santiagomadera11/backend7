using System.Collections.Generic;
using System.Threading.Tasks;
using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface ICategoriaService
    {
        Task<List<CategoriaDto>> ObtenerTodos();                 
        Task<List<CategoriaDto>> ObtenerTodosConInactivos();      
        Task<CategoriaDto?> ObtenerPorId(int id);
        Task<CategoriaDto> Crear(CategoriaCreateDto dto);
        Task<CategoriaDto> Actualizar(CategoriaUpdateDto dto);
        Task<bool> Eliminar(int id);
        Task<bool> CambiarEstado(int id, bool estado);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repo;
        public CategoriaService(ICategoriaRepository repo) => _repo = repo;

        public Task<List<CategoriaDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<List<CategoriaDto>> ObtenerTodosConInactivos() => _repo.ObtenerTodosConInactivos();
        public Task<CategoriaDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<CategoriaDto> Crear(CategoriaCreateDto dto) => _repo.Crear(dto);
        public Task<CategoriaDto> Actualizar(CategoriaUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<bool> CambiarEstado(int id, bool estado) => _repo.CambiarEstado(id, estado);
    }
}