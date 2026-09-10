using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.Business.Services;
using Syspharma.Domain.DTOs;
using System.Threading.Tasks;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _service;
        public NotificacionController(INotificacionService service) => _service = service;

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var result = await _service.ObtenerPorUsuario(usuarioId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] NotificacionCreateDto dto)
        {
            var result = await _service.Crear(dto);
            return Ok(result);
        }

        [HttpPatch("{id}/leida")]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            var ok = await _service.MarcarLeida(id);
            if (!ok) return NotFound(new { message = "Notificación no encontrada" });
            return Ok(new { message = "Notificación marcada como leída" });
        }

        [HttpPatch("{usuarioId}/leer-todas")]
        public async Task<IActionResult> MarcarTodasLeidas(int usuarioId)
        {
            await _service.MarcarTodasLeidas(usuarioId);
            return Ok(new { message = "Notificaciones marcadas como leídas" });
        }
    }
}
