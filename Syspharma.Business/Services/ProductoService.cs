using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> ObtenerTodos();
        Task<ProductoDto?> ObtenerPorId(int id);
        Task<ProductoDto> Crear(ProductoCreateDto dto);
        Task<ProductoDto> Actualizar(ProductoUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
        Task<List<ProductoDto>> ProximosAVencer(int dias);
    }

    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repository;
        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        // Delegación directa al repositorio unificado
        public Task<List<ProductoDto>> ObtenerTodos() => _repository.ObtenerTodos();
        public Task<ProductoDto?> ObtenerPorId(int id) => _repository.ObtenerPorId(id);
        public Task<ProductoDto> Crear(ProductoCreateDto dto) => _repository.Crear(dto);
        public Task<ProductoDto> Actualizar(ProductoUpdateDto dto) => _repository.Actualizar(dto);
        public Task<bool> CambiarEstado(int id, bool estado) => _repository.CambiarEstado(id, estado);
        public Task<bool> Eliminar(int id) => _repository.Eliminar(id);
        public Task<List<ProductoDto>> ProximosAVencer(int dias) => _repository.ProximosAVencer(dias);
    }
}
