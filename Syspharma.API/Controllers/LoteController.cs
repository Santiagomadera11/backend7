using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/lotes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LoteController : ControllerBase
    {
        private readonly SyspharmaContext _context;

        public LoteController(SyspharmaContext context)
        {
            _context = context;
        }

        [HttpGet("{loteId}/pedidos")]
        public async Task<IActionResult> ObtenerPedidosPorLote(int loteId)
        {
            try
            {
                var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == loteId);
                if (!loteExiste) return NotFound(new { message = "Lote no encontrado" });

                var consumos = await _context.VentaDetalles
                    .Where(vd => vd.LoteId == loteId)
                    .Select(vd => new
                    {
                        Fecha = vd.Venta.FechaVenta,
                        Cliente = vd.Venta.ClienteNombre ?? "Consumidor Final",
                        CantidadTomada = vd.Cantidad,
                        Usuario = vd.Venta.Usuario.Nombre
                    })
                    .OrderByDescending(c => c.Fecha)
                    .ToListAsync();

                return Ok(consumos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
