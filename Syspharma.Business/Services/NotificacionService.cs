using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    public interface INotificacionService
    {
        Task<List<NotificacionDto>> ObtenerPorUsuario(int usuarioId);
        Task<NotificacionDto> Crear(NotificacionCreateDto dto);
        Task<bool> MarcarLeida(int id);
        Task MarcarTodasLeidas(int usuarioId);
    }

    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _repository;
        public NotificacionService(INotificacionRepository repository) => _repository = repository;

        public Task<List<NotificacionDto>> ObtenerPorUsuario(int usuarioId) => _repository.ObtenerPorUsuario(usuarioId);
        public Task<NotificacionDto> Crear(NotificacionCreateDto dto) => _repository.Crear(dto);
        public Task<bool> MarcarLeida(int id) => _repository.MarcarLeida(id);
        public Task MarcarTodasLeidas(int usuarioId) => _repository.MarcarTodasLeidas(usuarioId);
    }
}
