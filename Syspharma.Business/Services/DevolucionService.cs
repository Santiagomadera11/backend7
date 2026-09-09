using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IDevolucionService
    {
        Task<List<DevolucionDto>> ObtenerTodos();
        Task<DevolucionDto?> ObtenerPorId(int id);
        Task<DevolucionDto?> ObtenerPorVentaId(int ventaId);
        Task<DevolucionDto> Crear(DevolucionCreateDto dto);
        Task<DevolucionDto> Gestionar(int id, DevolucionGestionarDto dto);
        Task<List<EstadoDevolucionDto>> ObtenerEstados();
    }

    public class DevolucionService : IDevolucionService
    {
        private readonly IDevolucionRepository _repo;
        public DevolucionService(IDevolucionRepository repo) => _repo = repo;

        public Task<List<DevolucionDto>> ObtenerTodos() => _repo.ObtenerTodos();
        public Task<DevolucionDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<DevolucionDto?> ObtenerPorVentaId(int ventaId) => _repo.ObtenerPorVentaId(ventaId);
        public Task<DevolucionDto> Crear(DevolucionCreateDto dto) => _repo.Crear(dto);
        public Task<DevolucionDto> Gestionar(int id, DevolucionGestionarDto dto) => _repo.Gestionar(id, dto);
        public Task<List<EstadoDevolucionDto>> ObtenerEstados() => _repo.ObtenerEstados();
    }
}
