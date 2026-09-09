using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly SyspharmaContext _context;

        public TipoDocumentoController(SyspharmaContext context) => _context = context;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodos()
        {
            var tipos = await _context.TiposDocumentos
                .Where(t => t.Estado == true)
                .OrderBy(t => t.Nombre)
                .Select(t => new { t.Id, t.Nombre, t.Estado, t.FechaCreacion })
                .ToListAsync();
            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var tipo = await _context.TiposDocumentos.FindAsync(id);
            if (tipo == null) return NotFound(new { message = "Tipo de documento no encontrado" });
            return Ok(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] TipoDocumentoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio" });

            if (await _context.TiposDocumentos.AnyAsync(t => t.Nombre == dto.Nombre))
                return BadRequest(new { message = "Ya existe un tipo de documento con ese nombre" });

            var tipo = new TiposDocumento
            {
                Nombre = dto.Nombre,
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            _context.TiposDocumentos.Add(tipo);
            await _context.SaveChangesAsync();
            return Ok(tipo);
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] TipoDocumentoUpdateDto dto)
        {
            var tipo = await _context.TiposDocumentos.FindAsync(dto.Id);
            if (tipo == null) return NotFound(new { message = "Tipo de documento no encontrado" });

            if (await _context.TiposDocumentos.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != dto.Id))
                return BadRequest(new { message = "Ya existe un tipo de documento con ese nombre" });

            tipo.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();
            return Ok(tipo);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            var tipo = await _context.TiposDocumentos.FindAsync(id);
            if (tipo == null) return NotFound(new { message = "Tipo de documento no encontrado" });
            tipo.Estado = estado;
            await _context.SaveChangesAsync();
            return Ok(tipo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var tipo = await _context.TiposDocumentos.FindAsync(id);
            if (tipo == null) return NotFound(new { message = "Tipo de documento no encontrado" });

            var enUso = await _context.Usuarios.AnyAsync(u => u.TipoDocumentoId == id) ||
                        await _context.Proveedores.AnyAsync(p => p.TipoDocumentoId == id);
            if (enUso)
                return BadRequest(new { message = "No se puede eliminar porque está en uso" });

            _context.TiposDocumentos.Remove(tipo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Eliminado correctamente" });
        }
    }

    public class TipoDocumentoDto
    {
        public string Nombre { get; set; } = null!;
    }

    public class TipoDocumentoUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}