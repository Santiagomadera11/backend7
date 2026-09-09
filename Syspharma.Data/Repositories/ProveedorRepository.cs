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
    // INTERFAZ INCLUIDA
    public interface IProveedorRepository
    {
        Task<List<ProveedorDto>> ObtenerTodos();
        Task<ProveedorDto?> ObtenerPorId(int id);
        Task<ProveedorDto> Crear(ProveedorCreateDto dto);
        Task<ProveedorDto> Actualizar(ProveedorUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
    }

    public class ProveedorRepository : IProveedorRepository
    {
        private readonly SyspharmaContext _context;
        public ProveedorRepository(SyspharmaContext context) => _context = context;

        private static ProveedorDto MapDto(Proveedore p) => new ProveedorDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Contacto = p.Contacto,
            Email = p.Email,
            Telefono = p.Telefono,
            Direccion = p.Direccion,
            TipoDocumento = p.TipoDocumento?.Nombre,
            TipoDocumentoId = p.TipoDocumentoId,
            Documento = p.Documento,
            Estado = p.Estado?.Nombre == "Activo" || p.EstadoId == null,
            EstadoId = p.EstadoId,
            FechaCreacion = p.FechaCreacion
        };

        public async Task<List<ProveedorDto>> ObtenerTodos() =>
            (await _context.Proveedores
                .Include(p => p.TipoDocumento)
                .Include(p => p.Estado)
                .ToListAsync()).Select(MapDto).ToList();

        public async Task<ProveedorDto?> ObtenerPorId(int id)
        {
            var p = await _context.Proveedores
                .Include(p => p.TipoDocumento)
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(p => p.Id == id);
            return p == null ? null : MapDto(p);
        }

        public async Task<ProveedorDto> Crear(ProveedorCreateDto dto)
        {
            if (await _context.Proveedores.AnyAsync(p => p.Nombre == dto.Nombre))
                throw new Exception("Ya existe un proveedor con ese nombre");

            var estadoActivo = await _context.EstadosProveedors.FirstOrDefaultAsync(e => e.Nombre == "Activo");

            var p = new Proveedore
            {
                Nombre = dto.Nombre,
                Contacto = dto.Contacto,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                TipoDocumentoId = dto.TipoDocumentoId,
                Documento = dto.Documento,
                EstadoId = estadoActivo?.Id,
                FechaCreacion = DateTime.Now
            };
            _context.Proveedores.Add(p);
            await _context.SaveChangesAsync();

            await _context.Entry(p).Reference(x => x.TipoDocumento).LoadAsync();
            await _context.Entry(p).Reference(x => x.Estado).LoadAsync();
            return MapDto(p);
        }

        public async Task<ProveedorDto> Actualizar(ProveedorUpdateDto dto)
        {
            var p = await _context.Proveedores
                .Include(x => x.TipoDocumento)
                .Include(x => x.Estado)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (p == null) throw new Exception("Proveedor no encontrado");

            p.Nombre = dto.Nombre;
            p.Contacto = dto.Contacto;
            p.Email = dto.Email;
            p.Telefono = dto.Telefono;
            p.Direccion = dto.Direccion;
            p.TipoDocumentoId = dto.TipoDocumentoId;
            p.Documento = dto.Documento;

            await _context.SaveChangesAsync();
            return MapDto(p);
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) throw new Exception("Proveedor no encontrado");

            var nombreEstado = estado ? "Activo" : "Inactivo";
            var estadoObj = await _context.EstadosProveedors.FirstOrDefaultAsync(e => e.Nombre == nombreEstado);
            p.EstadoId = estadoObj?.Id;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) throw new Exception("Proveedor no encontrado");
            var tieneProductos = await _context.Productos.AnyAsync(prod => prod.ProveedorId == id);
            if (tieneProductos)
                throw new Exception("No se puede eliminar el proveedor porque tiene productos asociados");
            _context.Proveedores.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}