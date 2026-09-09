using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Syspharma.Business.Services;
using Syspharma.Domain.DTOs;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolMaestroController : ControllerBase
    {
        private readonly IRolMaestroService _service;
        public RolMaestroController(IRolMaestroService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.ObtenerTodos());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.ObtenerPorId(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolMaestroDto dto) => Ok(await _service.Crear(dto));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RolMaestroDto dto) => Ok(await _service.Actualizar(dto));

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] RolMaestroEstadoDto dto)
        {
            try
            {
                var resultado = await _service.CambiarEstado(id, dto.Estado);
                if (!resultado) return NotFound(new { message = "El rol no existe" });

                return Ok(new { message = dto.Estado ? "Rol activado correctamente" : "Rol desactivado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _service.Eliminar(id);
                if (!resultado) return NotFound(new { message = "El rol no existe" });

                return Ok(new { message = "Rol eliminado correctamente" });
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                var fullMessage = innerMessage.ToLower();

                if (fullMessage.Contains("the delete statement conflicted with the reference constraint") ||
                    fullMessage.Contains("fk_usuarios_roles") ||
                    fullMessage.Contains("foreign key") ||
                    (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547))
                {
                    return Conflict(new
                    {
                        message = "No se puede eliminar este rol porque está asignado a uno o más usuarios. Desvincule el rol de los usuarios antes de eliminarlo.",
                        errorCode = "ROLE_HAS_USERS"
                    });
                }

                return BadRequest(new { message = innerMessage });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/permisos")]
        public async Task<IActionResult> GetPermisos(int id) => Ok(await _service.ObtenerPermisos(id));

        [HttpPost("{id}/permisos")]
        public async Task<IActionResult> AssignPermisos(int id, [FromBody] List<string> permisos)
            => Ok(await _service.AsignarPermisos(id, permisos));
    }
}