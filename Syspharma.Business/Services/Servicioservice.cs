using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    public interface IServicioService
    {
        Task<List<ServicioDto>> ObtenerTodos();
        Task<ServicioDto?> ObtenerPorId(int id);
        Task<ServicioDto> Crear(ServicioCreateDto dto);
        Task<ServicioDto> Actualizar(ServicioUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
    }

    public class ServicioService : IServicioService
    {
        private readonly IServicioRepository _repo;
        public ServicioService(IServicioRepository repo) => _repo = repo;

        public Task<List<ServicioDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<ServicioDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<ServicioDto> Crear(ServicioCreateDto dto) => _repo.Crear(dto);
        public Task<ServicioDto> Actualizar(ServicioUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> CambiarEstado(int id, bool estado) => _repo.CambiarEstado(id, estado);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
    }
}