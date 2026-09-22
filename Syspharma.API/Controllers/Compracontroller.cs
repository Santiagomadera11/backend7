using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.Business.Services;
using Syspharma.Domain.DTOs;
using Syspharma.API.Filters;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _service;
        public CompraController(ICompraService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var compras = await _service.ObtenerTodos();
            return Ok(compras);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var compra = await _service.ObtenerPorId(id);
            return compra == null ? NotFound(new { message = "Compra no encontrada" }) : Ok(compra);
        }

        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados()
        {
            var estados = await _service.ObtenerEstados();
            return Ok(estados);
        }

        [HttpPost]
        [RequirePermission("purchase.create")]
        public async Task<IActionResult> Crear([FromBody] CompraCreateDto dto)
        {
            try
            {
                var compra = await _service.Crear(dto);
                return Ok(compra);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut]
        [RequirePermission("purchase.edit")]
        public async Task<IActionResult> Actualizar([FromBody] CompraUpdateDto dto)
        {
            try
            {
                var compra = await _service.Actualizar(dto);
                return Ok(compra);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/estado")]
        [RequirePermission("purchase.status")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int estadoId)
        {
            try
            {
                await _service.CambiarEstado(id, estadoId);
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [RequirePermission("purchase.delete")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _service.Eliminar(id);
                return Ok(new { message = "Compra eliminada correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}