using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syspharma.Data.Repositories
{
    public interface IMedicoRepository
    {
        Task<List<MedicoDto>> ObtenerTodos();
        Task<MedicoDto?> ObtenerPorId(int id);
        Task<MedicoDto> Crear(MedicoCreateDto dto);
        Task<MedicoDto> Actualizar(MedicoUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
    }

    public class MedicoRepository : IMedicoRepository
    {
        private readonly SyspharmaContext _context;
        public MedicoRepository(SyspharmaContext context) => _context = context;

        private static MedicoDto MapDto(Medico m) => new MedicoDto
        {
            Id = m.Id,
            Nombre = m.Nombre,
            Especialidad = m.Especialidad,
            Documento = m.Documento,
            Email = m.Email,
            Telefono = m.Telefono,
            DiasLaborales = m.DiasLaborales,
            HoraInicio = m.HoraInicio?.ToString("HH:mm"),
            HoraFin = m.HoraFin?.ToString("HH:mm"),
            Intervalo = m.Intervalo,
            Estado = m.Estado,
            FechaCreacion = m.FechaCreacion
        };

        public async Task<List<MedicoDto>> ObtenerTodos() =>
            (await _context.Medicos.ToListAsync()).Select(MapDto).ToList();

        public async Task<MedicoDto?> ObtenerPorId(int id)
        {
            var m = await _context.Medicos.FindAsync(id);
            return m == null ? null : MapDto(m);
        }

        public async Task<MedicoDto> Crear(MedicoCreateDto dto)
        {
            var m = new Medico
            {
                Nombre = dto.Nombre,
                Especialidad = dto.Especialidad,
                Documento = dto.Documento,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            _context.Medicos.Add(m);
            await _context.SaveChangesAsync();
            return MapDto(m);
        }

        public async Task<MedicoDto> Actualizar(MedicoUpdateDto dto)
        {
            var m = await _context.Medicos.FindAsync(dto.Id);
            if (m == null) throw new Exception("Médico no encontrado");
            m.Nombre = dto.Nombre;
            m.Especialidad = dto.Especialidad;
            m.Documento = dto.Documento;
            m.Email = dto.Email;
            m.Telefono = dto.Telefono;
            await _context.SaveChangesAsync();
            return MapDto(m);
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var m = await _context.Medicos.FindAsync(id);
            if (m == null) return false;
            m.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var m = await _context.Medicos.FindAsync(id);
            if (m == null) return false;

            var tieneCitas = await _context.Citas.AnyAsync(c => c.MedicoId == id);
            if (tieneCitas)
                throw new Exception("No se puede eliminar el médico porque tiene citas registradas. Desactívalo en su lugar.");

            _context.Medicos.Remove(m);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}