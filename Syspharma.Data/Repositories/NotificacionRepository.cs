using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface INotificacionRepository
    {
        Task<List<NotificacionDto>> ObtenerPorUsuario(int usuarioId);
        Task<NotificacionDto> Crear(NotificacionCreateDto dto);
        Task<bool> MarcarLeida(int id);
        Task MarcarTodasLeidas(int usuarioId);
    }

    public class NotificacionRepository : INotificacionRepository
    {
        private readonly SyspharmaContext _context;
        public NotificacionRepository(SyspharmaContext context) => _context = context;

        private static NotificacionDto ToDto(Notificacion n) => new NotificacionDto
        {
            Id = n.Id,
            Tipo = n.Tipo,
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            Path = n.Path,
            Leida = n.Leida,
            FechaCreacion = n.FechaCreacion,
        };

        public async Task<List<NotificacionDto>> ObtenerPorUsuario(int usuarioId)
        {
            var lista = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.FechaCreacion)
                .Take(100)
                .ToListAsync();
            return lista.Select(ToDto).ToList();
        }

        public async Task<NotificacionDto> Crear(NotificacionCreateDto dto)
        {
            var n = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                Tipo = dto.Tipo,
                Titulo = dto.Titulo,
                Mensaje = dto.Mensaje,
                Path = dto.Path,
                Leida = false,
                FechaCreacion = DateTime.Now,
            };
            _context.Notificaciones.Add(n);
            await _context.SaveChangesAsync();
            return ToDto(n);
        }

        public async Task<bool> MarcarLeida(int id)
        {
            var n = await _context.Notificaciones.FindAsync(id);
            if (n == null) return false;
            n.Leida = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MarcarTodasLeidas(int usuarioId)
        {
            var pendientes = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .ToListAsync();
            foreach (var n in pendientes) n.Leida = true;
            await _context.SaveChangesAsync();
        }
    }
}
