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
    public class DevolucionController : ControllerBase
    {
        private readonly IDevolucionService _service;
        public DevolucionController(IDevolucionService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos() =>
            Ok(await _service.ObtenerTodos());

        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados() =>
            Ok(await _service.ObtenerEstados());

        [HttpGet("venta/{ventaId}")]
        public async Task<IActionResult> ObtenerPorVenta(int ventaId)
        {
            var result = await _service.ObtenerPorVentaId(ventaId);
            return result == null ? NotFound(new { message = "No hay devolución para esta venta" }) : Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _service.ObtenerPorId(id);
            return result == null ? NotFound(new { message = "Devolución no encontrada" }) : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] DevolucionCreateDto dto)
        {
            try { return Ok(await _service.Crear(dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/gestionar")]
        public async Task<IActionResult> Gestionar(int id, [FromBody] DevolucionGestionarDto dto)
        {
            try { return Ok(await _service.Gestionar(id, dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
