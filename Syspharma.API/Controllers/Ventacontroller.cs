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
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _service;
        public VentaController(IVentaService service) => _service = service;

        [HttpGet]
        [RequirePermission("sales.view")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var ventas = await _service.ObtenerTodos();
            return Ok(ventas);
        }

        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados()
        {
            var estados = await _service.ObtenerEstados();
            return Ok(estados);
        }

        [HttpGet("{id}")]
        [RequirePermission("sales.view")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var venta = await _service.ObtenerPorId(id);
            return venta == null ? NotFound(new { message = "Venta no encontrada" }) : Ok(venta);
        }

        [HttpPost]
        [RequirePermission("sales.create")]
        public async Task<IActionResult> Crear([FromBody] VentaCreateDto dto)
        {
            try
            {
                var venta = await _service.Crear(dto);
                return Ok(venta);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut]
        [RequirePermission("sales.create")]
        public async Task<IActionResult> Actualizar([FromBody] VentaUpdateDto dto)
        {
            try
            {
                var venta = await _service.Actualizar(dto);
                return Ok(venta);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/estado")]
        [RequirePermission("sales.create")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int estadoId)
        {
            try
            {
                await _service.CambiarEstado(id, estadoId);
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/anular")]
        [RequirePermission("sales.cancel")]
        public async Task<IActionResult> Anular(int id)
        {
            try
            {
                var result = await _service.Anular(id);
                if (!result) return NotFound(new { message = "Venta no encontrada" });
                return Ok(new { message = "Venta anulada correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [RequirePermission("sales.cancel")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _service.Eliminar(id);
                return Ok(new { message = "Venta eliminada correctamente" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
