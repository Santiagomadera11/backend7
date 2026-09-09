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
    public class CategoriaServicioController : ControllerBase
    {
        private readonly SyspharmaContext _context;

        public CategoriaServicioController(SyspharmaContext context) => _context = context;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodos()
        {
            var categorias = await _context.CategoriaServicios
                .Where(c => c.Estado == true)
                .OrderBy(c => c.Nombre)
                .Select(c => new { c.Id, c.Nombre, c.Descripcion, c.Estado, c.FechaCreacion })
                .ToListAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var categoria = await _context.CategoriaServicios.FindAsync(id);
            if (categoria == null) return NotFound(new { message = "Categoría de servicio no encontrada" });
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CategoriaServicioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio" });

            if (await _context.CategoriaServicios.AnyAsync(c => c.Nombre == dto.Nombre))
                return BadRequest(new { message = "Ya existe una categoría con ese nombre" });

            var categoria = new CategoriaServicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            _context.CategoriaServicios.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] CategoriaServicioUpdateDto dto)
        {
            var categoria = await _context.CategoriaServicios.FindAsync(dto.Id);
            if (categoria == null) return NotFound(new { message = "Categoría de servicio no encontrada" });

            if (await _context.CategoriaServicios.AnyAsync(c => c.Nombre == dto.Nombre && c.Id != dto.Id))
                return BadRequest(new { message = "Ya existe una categoría con ese nombre" });

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;
            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            var categoria = await _context.CategoriaServicios.FindAsync(id);
            if (categoria == null) return NotFound(new { message = "Categoría de servicio no encontrada" });
            categoria.Estado = estado;
            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var categoria = await _context.CategoriaServicios.FindAsync(id);
            if (categoria == null) return NotFound(new { message = "Categoría de servicio no encontrada" });

            var enUso = await _context.Servicios.AnyAsync(s => s.CategoriaId == id);
            if (enUso)
                return BadRequest(new { message = "No se puede eliminar porque tiene servicios asociados" });

            _context.CategoriaServicios.Remove(categoria);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Eliminado correctamente" });
        }
    }

    public class CategoriaServicioDto
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }

    public class CategoriaServicioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}