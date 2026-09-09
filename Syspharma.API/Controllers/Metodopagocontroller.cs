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
    public class MetodoPagoController : ControllerBase
    {
        private readonly SyspharmaContext _context;

        public MetodoPagoController(SyspharmaContext context) => _context = context;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodos()
        {
            var metodos = await _context.MetodosPagos
                .Where(m => m.Estado == true)
                .OrderBy(m => m.Nombre)
                .Select(m => new { m.Id, m.Nombre, m.Estado, m.FechaCreacion })
                .ToListAsync();
            return Ok(metodos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var metodo = await _context.MetodosPagos.FindAsync(id);
            if (metodo == null) return NotFound(new { message = "Método de pago no encontrado" });
            return Ok(metodo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] MetodoPagoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio" });

            if (await _context.MetodosPagos.AnyAsync(m => m.Nombre == dto.Nombre))
                return BadRequest(new { message = "Ya existe un método de pago con ese nombre" });

            var metodo = new MetodosPago
            {
                Nombre = dto.Nombre,
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            _context.MetodosPagos.Add(metodo);
            await _context.SaveChangesAsync();
            return Ok(metodo);
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] MetodoPagoUpdateDto dto)
        {
            var metodo = await _context.MetodosPagos.FindAsync(dto.Id);
            if (metodo == null) return NotFound(new { message = "Método de pago no encontrado" });

            if (await _context.MetodosPagos.AnyAsync(m => m.Nombre == dto.Nombre && m.Id != dto.Id))
                return BadRequest(new { message = "Ya existe un método de pago con ese nombre" });

            metodo.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();
            return Ok(metodo);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            var metodo = await _context.MetodosPagos.FindAsync(id);
            if (metodo == null) return NotFound(new { message = "Método de pago no encontrado" });
            metodo.Estado = estado;
            await _context.SaveChangesAsync();
            return Ok(metodo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var metodo = await _context.MetodosPagos.FindAsync(id);
            if (metodo == null) return NotFound(new { message = "Método de pago no encontrado" });

            var enUso = await _context.Ventas.AnyAsync(v => v.MetodoPagoId == id) ||
                        await _context.Pedidos.AnyAsync(p => p.MetodoPagoId == id);
            if (enUso)
                return BadRequest(new { message = "No se puede eliminar porque está en uso" });

            _context.MetodosPagos.Remove(metodo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Eliminado correctamente" });
        }
    }

    public class MetodoPagoDto
    {
        public string Nombre { get; set; } = null!;
    }

    public class MetodoPagoUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}