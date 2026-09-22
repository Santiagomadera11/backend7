using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Data.Repositories
{
    public interface IUsuarioRepository
    {
        Task<List<UsuarioDto>> ObtenerTodos();
        Task<UsuarioDto?> ObtenerPorId(int id);
        Task<UsuarioDto> Crear(UsuarioCreateDto dto);
        Task<UsuarioDto> Actualizar(UsuarioUpdateDto dto);
        Task<UsuarioDto> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
        Task<UsuarioDto> ActualizarAvatar(int id, string avatar);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SyspharmaContext _context;

        public UsuarioRepository(SyspharmaContext context)
        {
            _context = context;
        }

        private static UsuarioDto MapDto(Usuario u) => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email ?? string.Empty,
            Documento = u.Documento,
            TipoDocumento = u.TipoDocumento?.Nombre,
            TipoDocumentoId = u.TipoDocumentoId,
            Telefono = u.Telefono,
            Direccion = u.Direccion,
            RolNombre = u.Role?.Nombre ?? "",
            Avatar = u.Avatar,
            Estado = u.Estado,
            RolId = u.RoleId
        };

        public async Task<List<UsuarioDto>> ObtenerTodos()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Role)
                .Include(u => u.TipoDocumento)
                .ToListAsync();

            return usuarios.Select(MapDto).ToList();
        }

        public async Task<UsuarioDto?> ObtenerPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                .Include(u => u.TipoDocumento)
                .FirstOrDefaultAsync(u => u.Id == id && u.Estado == true);
            if (usuario == null) return null;
            return MapDto(usuario);
        }

        public async Task<UsuarioDto> Crear(UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("El email ya está registrado");

            if (!string.IsNullOrEmpty(dto.Documento) && await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento))
                throw new Exception("El número de documento ya está registrado por otro usuario.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                UserName = dto.Email,
                TipoDocumentoId = dto.TipoDocumentoId,
                Documento = dto.Documento,
                Telefono = dto.Telefono,
                RoleId = dto.RolId,
                FechaCreacion = DateTime.Now,
                Estado = true,
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await _context.Entry(usuario).Reference(u => u.Role).LoadAsync();
            await _context.Entry(usuario).Reference(u => u.TipoDocumento).LoadAsync();

            return MapDto(usuario);
        }

        public async Task<UsuarioDto> Actualizar(UsuarioUpdateDto dto)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.Id == dto.Id))
                throw new Exception("Usuario no encontrado");

            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != dto.Id))
                throw new Exception("El email ya está registrado por otro usuario.");

            if (!string.IsNullOrEmpty(dto.Documento) && await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento && u.Id != dto.Id))
                throw new Exception("El número de documento ya está registrado por otro usuario.");

            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                .Include(u => u.TipoDocumento)
                .FirstOrDefaultAsync(u => u.Id == dto.Id);
            if (usuario == null) throw new Exception("Usuario no encontrado");

            var esAdministrador = usuario.Role?.Nombre?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ?? false;
            if (!dto.Estado && esAdministrador)
                throw new Exception("No se puede desactivar a un usuario con rol Administrador.");

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;
            usuario.TipoDocumentoId = dto.TipoDocumentoId;
            usuario.Documento = dto.Documento;
            usuario.Telefono = dto.Telefono;
            usuario.Direccion = dto.Direccion;
            usuario.RoleId = dto.RolId;
            usuario.Estado = dto.Estado;
            usuario.Avatar = dto.Avatar ?? usuario.Avatar;

            await _context.SaveChangesAsync();
            return MapDto(usuario);
        }

        public async Task<UsuarioDto> CambiarEstado(int id, bool estado)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                .Include(u => u.TipoDocumento)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (!estado && (usuario.Role?.Nombre?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ?? false))
                throw new Exception("No se puede desactivar a un usuario con rol Administrador.");

            usuario.Estado = estado;
            await _context.SaveChangesAsync();
            return MapDto(usuario);
        }

        public async Task<bool> Eliminar(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            if (usuario.Role?.Nombre?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ?? false)
                throw new Exception("No se puede eliminar a un usuario con rol Administrador. Desactívalo primero si ya no debe tener acceso, o cámbiale el rol.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UsuarioDto> ActualizarAvatar(int id, string avatar)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                .Include(u => u.TipoDocumento)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            usuario.Avatar = avatar;
            await _context.SaveChangesAsync();

            return MapDto(usuario);
        }
    }
}