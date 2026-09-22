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
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _service;
        public ProveedorController(IProveedorService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos() => Ok(await _service.ObtenerTodos());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var r = await _service.ObtenerPorId(id);
            return r == null ? NotFound(new { message = "Proveedor no encontrado" }) : Ok(r);
        }

        [HttpPost]
        [RequirePermission("suppliers.create")]
        public async Task<IActionResult> Crear([FromBody] ProveedorCreateDto dto)
        {
            try { return Ok(await _service.Crear(dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut]
        [RequirePermission("suppliers.edit")]
        public async Task<IActionResult> Actualizar([FromBody] ProveedorUpdateDto dto)
        {
            try { return Ok(await _service.Actualizar(dto)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPatch("{id}/estado")]
        [RequirePermission("suppliers.status")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            try { await _service.CambiarEstado(id, estado); return Ok(new { message = "Estado actualizado" }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [RequirePermission("suppliers.delete")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try { await _service.Eliminar(id); return Ok(new { message = "Proveedor eliminado correctamente" }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}