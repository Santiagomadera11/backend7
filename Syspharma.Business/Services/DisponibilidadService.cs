using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    public interface IDisponibilidadService
    {
        Task<List<HorarioDto>> ObtenerHorario(int medicoId);
        Task GuardarHorario(int medicoId, List<HorarioItemDto> horarios);
        Task<List<BloqueoDto>> ObtenerBloqueos(int medicoId);
        Task<BloqueoDto> CrearBloqueo(BloqueoCreateDto dto);
        Task<bool> EliminarBloqueo(int id);
        Task<List<string>> ObtenerSlots(int medicoId, DateOnly fecha);
    }

    public class DisponibilidadService : IDisponibilidadService
    {
        private readonly IDisponibilidadRepository _repo;
        public DisponibilidadService(IDisponibilidadRepository repo) => _repo = repo;

        public Task<List<HorarioDto>> ObtenerHorario(int medicoId) => _repo.ObtenerHorario(medicoId);
        public Task GuardarHorario(int medicoId, List<HorarioItemDto> horarios) => _repo.GuardarHorario(medicoId, horarios);
        public Task<List<BloqueoDto>> ObtenerBloqueos(int medicoId) => _repo.ObtenerBloqueos(medicoId);
        public Task<BloqueoDto> CrearBloqueo(BloqueoCreateDto dto) => _repo.CrearBloqueo(dto);
        public Task<bool> EliminarBloqueo(int id) => _repo.EliminarBloqueo(id);
        public Task<List<string>> ObtenerSlots(int medicoId, DateOnly fecha) => _repo.ObtenerSlots(medicoId, fecha);
    }
}
