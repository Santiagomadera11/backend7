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
    public interface ITurnoRepository
    {
        Task<List<TurnoDto>> ObtenerTodos();
        Task<TurnoDto?> ObtenerPorId(int id);
        Task<TurnoDto?> ObtenerTurnoActivo(int usuarioId);
        Task<TurnoDto> Abrir(TurnoAbrirDto dto);
        Task<TurnoDto> Cerrar(TurnoCerrarDto dto);
        Task<bool> Eliminar(int id);
    }

    public class TurnoRepository : ITurnoRepository
    {
        private readonly SyspharmaContext _context;
        public TurnoRepository(SyspharmaContext context) => _context = context;

        private TurnoDto MapDto(Turno t) => new TurnoDto
        {
            Id = t.Id,
            UsuarioId = t.UsuarioId,
            UsuarioNombre = t.Usuario?.Nombre ?? "N/A",
            Estado = t.Estado.Trim(),
            MontoBase = t.MontoBase,
            MontoFinal = t.MontoFinal,
            TotalVentas = t.TotalVentas,
            TotalGastos = t.TotalGastos,
            ResumenVentas = t.ResumenVentas,
            ResumenServicios = t.ResumenServicios,
            ResumenErroresCaja = t.ResumenErroresCaja,
            Diferencia = t.Diferencia,
            Notas = t.Notas,
            FechaApertura = t.FechaApertura,
            FechaCierre = t.FechaCierre
        };

        public async Task<List<TurnoDto>> ObtenerTodos()
        {
            var turnos = await _context.Turnos.Include(t => t.Usuario).OrderByDescending(t => t.FechaApertura).ToListAsync();
            return turnos.Select(MapDto).ToList();
        }

        public async Task<TurnoDto?> ObtenerPorId(int id)
        {
            var t = await _context.Turnos.Include(t => t.Usuario).FirstOrDefaultAsync(t => t.Id == id);
            return t == null ? null : MapDto(t);
        }

        public async Task<TurnoDto?> ObtenerTurnoActivo(int usuarioId)
        {
            var turno = await _context.Turnos.Include(t => t.Usuario)
                .Where(t => t.Estado.Contains("activo"))
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();
            return turno == null ? null : MapDto(turno);
        }

        public async Task<TurnoDto> Abrir(TurnoAbrirDto dto)
        {
            var existe = await _context.Turnos.AnyAsync(t => t.Estado.Contains("activo"));
            if (existe) throw new Exception("Ya hay una caja abierta.");

            var turno = new Turno
            {
                UsuarioId = dto.UsuarioId,
                MontoBase = dto.MontoBase,
                Estado = "activo",
                FechaApertura = DateTime.Now,
                TotalVentas = 0,
                TotalGastos = 0
            };
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            return await ObtenerPorId(turno.Id) ?? MapDto(turno);
        }

        public async Task<TurnoDto> Cerrar(TurnoCerrarDto dto)
        {
            var turno = await _context.Turnos.FindAsync(dto.Id);
            if (turno == null) throw new Exception("Turno no encontrado");
            turno.Estado = "cerrado";
            turno.FechaCierre = DateTime.Now;
            turno.MontoFinal = dto.MontoFinal;
            await _context.SaveChangesAsync();
            return MapDto(turno);
        }

        public async Task<bool> Eliminar(int id)
        {
            var t = await _context.Turnos.FindAsync(id);
            if (t == null) return false;
            _context.Turnos.Remove(t);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}