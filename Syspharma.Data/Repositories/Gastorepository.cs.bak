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
    public interface IGastoRepository
    {
        Task<List<GastoDto>> ObtenerTodos();
        Task<List<GastoDto>> ObtenerPorTurno(int turnoId);
        Task<GastoDto?> ObtenerPorId(int id);
        Task<GastoDto> Crear(GastoCreateDto dto);
        Task<GastoDto> Actualizar(GastoUpdateDto dto);
        Task<bool> Eliminar(int id);
    }

    public class GastoRepository : IGastoRepository
    {
        private readonly SyspharmaContext _context;
        public GastoRepository(SyspharmaContext context) => _context = context;

        private static GastoDto MapDto(Gasto g) => new GastoDto
        {
            Id = g.Id,
            TurnoId = g.TurnoId,
            UsuarioId = g.UsuarioId,
            UsuarioNombre = g.Usuario?.Nombre ?? "",
            Concepto = g.Concepto,
            Descripcion = g.Descripcion,
            Monto = g.Monto,
            Categoria = g.Categoria,
            Comprobante = g.Comprobante,
            FechaGasto = g.FechaGasto
        };

        public async Task<List<GastoDto>> ObtenerTodos() =>
            (await _context.Gastos.Include(g => g.Usuario).OrderByDescending(g => g.FechaGasto).ToListAsync()).Select(MapDto).ToList();

        public async Task<List<GastoDto>> ObtenerPorTurno(int turnoId) =>
            (await _context.Gastos.Include(g => g.Usuario).Where(g => g.TurnoId == turnoId).ToListAsync()).Select(MapDto).ToList();

        public async Task<GastoDto?> ObtenerPorId(int id)
        {
            var g = await _context.Gastos.Include(g => g.Usuario).FirstOrDefaultAsync(x => x.Id == id);
            return g == null ? null : MapDto(g);
        }

        public async Task<GastoDto> Crear(GastoCreateDto dto)
        {
            var gasto = new Gasto
            {
                TurnoId = dto.TurnoId,
                UsuarioId = dto.UsuarioId,
                Concepto = dto.Concepto,
                Monto = dto.Monto,
                Categoria = dto.Categoria,
                FechaGasto = DateTime.Now
            };
            _context.Gastos.Add(gasto);
            await _context.SaveChangesAsync();
            return MapDto(gasto);
        }

        public async Task<GastoDto> Actualizar(GastoUpdateDto dto) { return null; }
        public async Task<bool> Eliminar(int id) { return true; }
    }
}