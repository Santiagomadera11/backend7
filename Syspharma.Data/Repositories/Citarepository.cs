using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{

    public interface ICitaRepository
    {
        Task<List<CitaDto>> ObtenerTodos(DateTime? desde = null);
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
            VentaId = c.VentaId,        // ← AGREGADO
            Fecha = c.Fecha == default ? DateTime.Now.ToString("yyyy-MM-dd") : c.Fecha.ToString("yyyy-MM-dd"),
            Hora = c.Hora == default ? "00:00" : c.Hora.ToString(@"HH\:mm"),
            Notas = c.Notas,
            FechaCreacion = c.FechaCreacion
        };

        // "desde" filtra en SQL (para el feed de notificaciones, que solo necesita
        // las citas creadas después del último visto en vez de traer la tabla entera).
        public async Task<List<CitaDto>> ObtenerTodos(DateTime? desde = null)
        {
            var query = _context.Citas
                .Include(c => c.Medico).Include(c => c.Estado)
                .Include(c => c.Servicio).Include(c => c.Usuario)
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(c => c.FechaCreacion > desde.Value);

            var citas = await query.OrderByDescending(c => c.Fecha).ToListAsync();
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

            // El selector de horarios (ObtenerSlots) ya oculta las horas ocupadas, pero eso es
            // solo la UI — sin este chequeo acá, dos solicitudes casi simultáneas (o una llamada
            // directa a la API) podían agendar dos pacientes con el mismo médico a la misma hora.
            // "Cancelada"/"No Asistió" no cuentan porque liberan el horario.
            var yaOcupado = await _context.Citas.AnyAsync(c =>
                c.MedicoId == dto.MedicoId && c.Fecha == fechaCita && c.Hora == horaCita &&
                c.EstadoId != 5 && c.EstadoId != 6);
            if (yaOcupado)
                throw new Exception("Ya existe una cita agendada con este médico en esa fecha y hora.");

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
            var cita = await _context.Citas.FindAsync(dto.Id) ?? throw new Exception("No existe");

            var fechaCita = DateOnly.Parse(dto.Fecha);
            var horaCita = TimeOnly.Parse(dto.Hora);

            // Solo se valida "no puede ser pasado" si realmente se está reprogramando
            // (fecha/hora distintas a las que ya tenía). Antes se exigía siempre, así que
            // era imposible editar cualquier otro campo (teléfono, notas, servicio) de una
            // cita cuya fecha ya pasó naturalmente.
            if (fechaCita != cita.Fecha || horaCita != cita.Hora)
            {
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

                // Igual que en Crear: bloquear reprogramar hacia una hora que ya tiene otra
                // cita agendada con ese médico.
                var yaOcupado = await _context.Citas.AnyAsync(c =>
                    c.Id != dto.Id && c.MedicoId == dto.MedicoId && c.Fecha == fechaCita && c.Hora == horaCita &&
                    c.EstadoId != 5 && c.EstadoId != 6);
                if (yaOcupado)
                    throw new Exception("Ya existe una cita agendada con este médico en esa fecha y hora.");
            }

            // Si cambia el servicio, se vuelve a tomar su precio/nombre histórico (igual
            // que en Crear); si no cambia, se preservan los que ya tenía.
            if (dto.ServicioId.HasValue && dto.ServicioId != cita.ServicioId)
            {
                var servicio = await _context.Servicios.FindAsync(dto.ServicioId.Value);
                cita.ServicioNombre = servicio?.Nombre;
                cita.Precio = servicio?.Precio;
            }

            cita.MedicoId = dto.MedicoId;
            cita.PacienteNombre = dto.PacienteNombre;
            cita.PacienteDocumento = dto.PacienteDocumento;
            cita.PacienteTelefono = dto.PacienteTelefono;
            cita.PacienteEmail = dto.PacienteEmail;
            cita.ServicioId = dto.ServicioId;
            cita.EstadoId = dto.EstadoId;
            cita.Fecha = fechaCita;
            cita.Hora = horaCita;
            cita.Notas = dto.Notas;
            await _context.SaveChangesAsync();
            return await ObtenerPorId(cita.Id) ?? throw new Exception("Error");
        }

        public async Task<bool> CambiarEstado(int id, int estadoId)
        {
            var c = await _context.Citas.FindAsync(id);
            if (c == null) return false;

            // "Pagada" no se puede fijar a mano por acá: solo debe alcanzarse cuando
            // VentaService.Crear procesa un cobro real (ver el vínculo Cita↔Venta). Si no,
            // cualquiera podría marcar una cita como pagada sin que exista ninguna venta.
            var estadoDestino = await _context.EstadosCita.FindAsync(estadoId);
            if (estadoDestino != null && estadoDestino.Nombre == "Pagada" && c.VentaId == null)
                throw new Exception("El estado 'Pagada' no se puede asignar manualmente: se marca automáticamente al cobrar la cita mediante una venta.");

            c.EstadoId = estadoId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var c = await _context.Citas.FindAsync(id);
            if (c == null) return false;

            if (c.VentaId.HasValue)
                throw new Exception("No se puede eliminar una cita que ya fue pagada: perderías la trazabilidad de a qué cita corresponde esa venta.");

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