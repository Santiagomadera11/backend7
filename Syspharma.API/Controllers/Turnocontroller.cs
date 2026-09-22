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
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoService _service;

        public TurnoController(ITurnoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var turnos = await _service.ObtenerTodos();
            return Ok(turnos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var t = await _service.ObtenerPorId(id);
            if (t == null)
                return NotFound(new { message = "Turno no encontrado" });

            return Ok(t);
        }

        [HttpGet("activo/{usuarioId}")]
        public async Task<IActionResult> ObtenerTurnoActivo(int usuarioId)
        {
            var t = await _service.ObtenerTurnoActivo(usuarioId);
            if (t == null)
                return NotFound(new { message = "No hay una caja/turno activo para este usuario" });

            return Ok(t);
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> Abrir([FromBody] TurnoAbrirDto dto)
        {
            try
            {
                var nuevoTurno = await _service.Abrir(dto);
                return Ok(nuevoTurno);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("cerrar")]
        public async Task<IActionResult> Cerrar([FromBody] TurnoCerrarDto dto)
        {
            try
            {
                var turnoCerrado = await _service.Cerrar(dto);
                return Ok(turnoCerrado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _service.Eliminar(id);
                if (!resultado) return NotFound(new { message = "Turno no encontrado" });

                return Ok(new { message = "Turno eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}