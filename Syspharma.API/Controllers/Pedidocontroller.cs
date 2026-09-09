using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.Business.Services;
using Syspharma.Domain.DTOs;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _service;
        public PedidoController(IPedidoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos() => Ok(await _service.ObtenerTodos());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var pedido = await _service.ObtenerPorId(id);
            return pedido == null ? NotFound(new { message = "Pedido no encontrado" }) : Ok(pedido);
        }

        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados() => Ok(await _service.ObtenerEstados());

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PedidoCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
            try
            {
                var pedido = await _service.Crear(dto);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] PedidoUpdateDto dto)
        {
            try
            {
                var pedido = await _service.Actualizar(dto);
                return Ok(pedido);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/estado")]
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
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _service.Eliminar(id);
                return Ok(new { message = "Pedido eliminado correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}