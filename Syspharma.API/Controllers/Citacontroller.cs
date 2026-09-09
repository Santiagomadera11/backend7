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
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _service;
        public CitaController(ICitaService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos() => Ok(await _service.ObtenerTodos());

        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados() => Ok(await _service.ObtenerEstados());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var c = await _service.ObtenerPorId(id);
            return c == null ? NotFound() : Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CitaCreateDto dto)
        {
            try
            {
                return Ok(await _service.Crear(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] CitaUpdateDto dto)
        {
            try
            {
                return Ok(await _service.Actualizar(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int estadoId)
        {
            var res = await _service.CambiarEstado(id, estadoId);
            return res ? Ok(new { message = "Estado actualizado" }) : NotFound();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var res = await _service.Eliminar(id);
            return res ? Ok(new { message = "Cita eliminada correctamente" }) : NotFound();
        }
    }
}