using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.Business.Services;
using Syspharma.Domain.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Syspharma.API.Filters;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;
        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos([FromQuery] string? estado = null)
        {
            if (string.IsNullOrEmpty(estado) || estado.Equals("activo", StringComparison.OrdinalIgnoreCase))
            {
                var result = await _service.ObtenerTodos();
                return Ok(result);
            }
            else if (estado.Equals("todos", StringComparison.OrdinalIgnoreCase))
            {
                var result = await _service.ObtenerTodosConInactivos();
                return Ok(result);
            }
            else if (estado.Equals("inactivo", StringComparison.OrdinalIgnoreCase))
            {
                var all = await _service.ObtenerTodosConInactivos();
                var result = all.Where(c => c.Estado == false).ToList();
                return Ok(result);
            }
            else
            {
                var result = await _service.ObtenerTodos();
                return Ok(result);
            }
        }

        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var result = await _service.ObtenerTodosConInactivos();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _service.ObtenerPorId(id);
            if (result == null) return NotFound(new { message = "Categoría no encontrada" });
            return Ok(result);
        }

        [HttpPost]
        [RequirePermission("categories.create")]
        public async Task<IActionResult> Crear([FromBody] CategoriaCreateDto dto)
        {
            try
            {
                var result = await _service.Crear(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [RequirePermission("categories.edit")]
        public async Task<IActionResult> Actualizar([FromBody] CategoriaUpdateDto dto)
        {
            try
            {
                var result = await _service.Actualizar(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        [RequirePermission("categories.status")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            try
            {
                await _service.CambiarEstado(id, estado);
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [RequirePermission("categories.delete")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _service.Eliminar(id);
                return Ok(new { message = "Categoría eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
