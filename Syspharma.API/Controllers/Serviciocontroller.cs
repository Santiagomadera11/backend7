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
    public class ServicioController : ControllerBase
    {
        private readonly IServicioService _service;
        public ServicioController(IServicioService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos() => Ok(await _service.ObtenerTodos());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var s = await _service.ObtenerPorId(id);
            return s == null ? NotFound(new { message = "Servicio no encontrado" }) : Ok(s);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ServicioCreateDto dto)
        {
            try { return Ok(await _service.Crear(dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ServicioUpdateDto dto)
        {
            try { return Ok(await _service.Actualizar(dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            try { await _service.CambiarEstado(id, estado); return Ok(new { message = "Estado actualizado" }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try { await _service.Eliminar(id); return Ok(new { message = "Servicio eliminado correctamente" }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}