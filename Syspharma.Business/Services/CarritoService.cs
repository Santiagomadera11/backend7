using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    public interface ICarritoService
    {
        Task<List<CarritoItemDto>> ObtenerPorUsuario(int usuarioId);
        Task<List<CarritoItemDto>> UpsertItem(int usuarioId, CarritoItemUpsertDto dto);
        Task<List<CarritoItemDto>> EliminarItem(int usuarioId, int productoId, int? formaVentaId);
        Task VaciarCarrito(int usuarioId);
    }

    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _repository;
        public CarritoService(ICarritoRepository repository) => _repository = repository;

        public Task<List<CarritoItemDto>> ObtenerPorUsuario(int usuarioId) => _repository.ObtenerPorUsuario(usuarioId);
        public Task<List<CarritoItemDto>> UpsertItem(int usuarioId, CarritoItemUpsertDto dto) => _repository.UpsertItem(usuarioId, dto);
        public Task<List<CarritoItemDto>> EliminarItem(int usuarioId, int productoId, int? formaVentaId) => _repository.EliminarItem(usuarioId, productoId, formaVentaId);
        public Task VaciarCarrito(int usuarioId) => _repository.VaciarCarrito(usuarioId);
    }
}
