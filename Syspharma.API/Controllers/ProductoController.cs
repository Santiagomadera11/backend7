using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syspharma.Business.Services;
using Syspharma.Data.Context;
using Syspharma.Domain.DTOs;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _service;
        private readonly SyspharmaContext _context;

        public ProductoController(IProductoService service, SyspharmaContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _service.ObtenerTodos();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _service.ObtenerPorId(id);
            if (result == null) return NotFound(new { message = "Producto no encontrado" });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoCreateDto dto)
        {
            try
            {
                // El DTO recibido ya contiene opcionalmente las propiedades 'EsMedicamento' y 'Medicamento'
                var result = await _service.Crear(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ProductoUpdateDto dto)
        {
            try
            {
                // Permite actualizar los datos básicos y los detalles de medicamento en una sola llamada
                var result = await _service.Actualizar(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            try
            {
                await _service.CambiarEstado(id, estado);
                return Ok(new { message = "Estado actualizado correctamente" });
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
                await _service.Eliminar(id);
                return Ok(new { message = "Producto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("proximos-a-vencer")]
        [AllowAnonymous]
        public async Task<IActionResult> ProximosAVencer([FromQuery] int? dias = null)
        {
            try
            {
                int diasLimite;
                if (dias.HasValue)
                {
                    diasLimite = dias.Value;
                }
                else
                {
                    var config = await _context.Configuraciones
                        .FirstOrDefaultAsync(c => c.Clave == "dias_alerta_vencimiento");
                    diasLimite = int.TryParse(config?.Valor, out var d) ? d : 30;
                }

                var hoyDateOnly = DateOnly.FromDateTime(DateTime.Today);
                var limiteDateOnly = hoyDateOnly.AddDays(diasLimite);

                var productos = await _context.Productos
                    .Where(p => p.Estado &&
                                p.FechaVencimientoProxima != null &&
                                p.FechaVencimientoProxima <= limiteDateOnly &&
                                p.FechaVencimientoProxima >= hoyDateOnly)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.Stock,
                        FechaVencimiento = p.FechaVencimientoProxima,
                        DiasRestantes = EF.Functions.DateDiffDay(hoyDateOnly, p.FechaVencimientoProxima!.Value)
                    })
                    .OrderBy(p => p.FechaVencimiento)
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        [HttpGet("{id}/lotes")]
        public async Task<IActionResult> ObtenerLotesPorProducto(int id)
        {
            try
            {
                var productoExiste = await _context.Productos.AnyAsync(p => p.Id == id);
                if (!productoExiste) return NotFound(new { message = "Producto no encontrado" });

                var hoy = DateOnly.FromDateTime(DateTime.Today);
                var config = await _context.Configuraciones
                    .FirstOrDefaultAsync(c => c.Clave == "dias_alerta_vencimiento");
                int diasAlerta = int.TryParse(config?.Valor, out var d) ? d : 30;
                var limiteAlerta = hoy.AddDays(diasAlerta);

                var lotes = await _context.Lotes
                    .Where(l => l.ProductoId == id)
                    .Select(l => new
                    {
                        l.Id,
                        l.NumeroLote,
                        l.FechaVencimiento,
                        CantidadDisponible = l.Cantidad,
                        Estado = l.Cantidad == 0 ? "Agotado" :
                                 l.FechaVencimiento < hoy ? "Vencido" :
                                 l.FechaVencimiento <= limiteAlerta ? "Por vencer" : "Vigente"
                    })
                    .OrderBy(l => l.FechaVencimiento)
                    .ToListAsync();

                return Ok(lotes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
