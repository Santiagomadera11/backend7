using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.Business.Services;
using Syspharma.Domain.DTOs;
using System;
using System.Threading.Tasks;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _service;
        public CarritoController(ICarritoService service) => _service = service;

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var result = await _service.ObtenerPorUsuario(usuarioId);
            return Ok(result);
        }

        [HttpPut("{usuarioId}/items")]
        public async Task<IActionResult> UpsertItem(int usuarioId, [FromBody] CarritoItemUpsertDto dto)
        {
            try
            {
                var result = await _service.UpsertItem(usuarioId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{usuarioId}/items/{productoId}")]
        public async Task<IActionResult> EliminarItem(int usuarioId, int productoId, [FromQuery] int? formaVentaId)
        {
            var result = await _service.EliminarItem(usuarioId, productoId, formaVentaId);
            return Ok(result);
        }

        [HttpDelete("{usuarioId}")]
        public async Task<IActionResult> VaciarCarrito(int usuarioId)
        {
            await _service.VaciarCarrito(usuarioId);
            return Ok(new { message = "Carrito vaciado correctamente" });
        }
    }
}
