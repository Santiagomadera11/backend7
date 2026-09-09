using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{

    public interface ICitaRepository
    {
        Task<List<CitaDto>> ObtenerTodos();
        Task<CitaDto?> ObtenerPorId(int id);
        Task<CitaDto> Crear(CitaCreateDto dto);
        Task<CitaDto> Actualizar(CitaUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Eliminar(int id);

        Task<List<CitaEstadoDto>> ObtenerEstados(); 
    }

    public class CitaRepository : ICitaRepository
    {
        private readonly SyspharmaContext _context;
        public CitaRepository(SyspharmaContext context) => _context = context;

        private static CitaDto MapDto(Cita c) => new CitaDto
        {
            Id = c.Id,
            MedicoId = c.MedicoId,
            MedicoNombre = c.Medico?.Nombre ?? "Sin médico",
            PacienteNombre = c.PacienteNombre,
            PacienteDocumento = c.PacienteDocumento,
            PacienteTelefono = c.PacienteTelefono,
            PacienteEmail = c.PacienteEmail,
            ServicioId = c.ServicioId,
            ServicioNombre = c.Servicio?.Nombre ?? c.ServicioNombre ?? "Consulta",
            Precio = c.Precio ?? c.Servicio?.Precio ?? 0,
            EstadoId = c.EstadoId,
            EstadoNombre = c.Estado?.Nombre ?? "Pendiente",
            UsuarioId = c.UsuarioId,
            UsuarioNombre = c.Usuario?.Nombre,
            PedidoId = c.PedidoId,      // ← AGREGADO
            VentaId = c.VentaId,        // ← AGREGADO
            Fecha = c.Fecha == default ? DateTime.Now.ToString("yyyy-MM-dd") : c.Fecha.ToString("yyyy-MM-dd"),
            Hora = c.Hora == default ? "00:00" : c.Hora.ToString(@"HH\:mm"),
            Notas = c.Notas,
            FechaCreacion = c.FechaCreacion
        };

        public async Task<List<CitaDto>> ObtenerTodos()
        {
            var citas = await _context.Citas
                .Include(c => c.Medico).Include(c => c.Estado)
                .Include(c => c.Servicio).Include(c => c.Usuario)
                .OrderByDescending(c => c.Fecha).ToListAsync();
            return citas.Select(MapDto).ToList();
        }

        public async Task<CitaDto?> ObtenerPorId(int id)
        {
            var c = await _context.Citas
                .Include(c => c.Medico).Include(c => c.Estado)
                .Include(c => c.Servicio).Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);
            return c == null ? null : MapDto(c);
        }

        public async Task<CitaDto> Crear(CitaCreateDto dto)
        {
            var fechaCita = DateOnly.Parse(dto.Fecha);
            var horaCita = TimeOnly.Parse(dto.Hora);
            var fechaHoraCita = fechaCita.ToDateTime(horaCita);

            TimeZoneInfo colombiaZone;
            try
            {
                colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            }

            var horaActualColombia = TimeZoneInfo.ConvertTime(DateTime.UtcNow, colombiaZone);

            if (fechaHoraCita <= horaActualColombia)
                throw new Exception("No se puede agendar una cita en una hora o día ya pasado.");

            // --- NUEVO: Buscamos el servicio asociado para guardar su precio y nombre histórico ---
            var servicio = await _context.Servicios.FindAsync(dto.ServicioId);
            decimal? precioServicio = servicio?.Precio;
            string? nombreServicio = servicio?.Nombre;

            var cita = new Cita
            {
                MedicoId = dto.MedicoId,
                PacienteNombre = dto.PacienteNombre,
                PacienteDocumento = dto.PacienteDocumento,
                PacienteTelefono = dto.PacienteTelefono,
                PacienteEmail = dto.PacienteEmail,
                ServicioId = dto.ServicioId,

                // Guardamos el precio y el nombre histórico del servicio en la cita
                ServicioNombre = nombreServicio,
                Precio = precioServicio,

                UsuarioId = dto.UsuarioId > 0 ? dto.UsuarioId : null,
                EstadoId = 1,
                Fecha = fechaCita,
                Hora = horaCita,
                Notas = dto.Notas,
                FechaCreacion = DateTime.Now
            };
            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();
            return await ObtenerPorId(cita.Id) ?? MapDto(cita);
        }

        public async Task<CitaDto> Actualizar(CitaUpdateDto dto)
        {
            var fechaCita = DateOnly.Parse(dto.Fecha);
            var horaCita = TimeOnly.Parse(dto.Hora);
            var fechaHoraCita = fechaCita.ToDateTime(horaCita);

            TimeZoneInfo colombiaZone;
            try
            {
                colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            }

            var horaActualColombia = TimeZoneInfo.ConvertTime(DateTime.UtcNow, colombiaZone);

            if (fechaHoraCita <= horaActualColombia)
                throw new Exception("No se puede agendar una cita en una hora o día ya pasado.");

            var cita = await _context.Citas.FindAsync(dto.Id) ?? throw new Exception("No existe");
            cita.MedicoId = dto.MedicoId; cita.PacienteNombre = dto.PacienteNombre;
            cita.EstadoId = dto.EstadoId; cita.Fecha = fechaCita;
            cita.Hora = horaCita;
            await _context.SaveChangesAsync();
            return await ObtenerPorId(cita.Id) ?? throw new Exception("Error");
        }

        public async Task<bool> CambiarEstado(int id, int estadoId)
        {
            var c = await _context.Citas.FindAsync(id);
            if (c == null) return false;
            c.EstadoId = estadoId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var c = await _context.Citas.FindAsync(id);
            if (c == null) return false;
            _context.Citas.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CitaEstadoDto>> ObtenerEstados()
        {
            return await _context.EstadosCita
                .Select(e => new CitaEstadoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();
        }
    }
}