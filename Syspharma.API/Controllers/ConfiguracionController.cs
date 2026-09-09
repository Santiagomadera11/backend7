using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;

namespace Syspharma.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ConfiguracionController : ControllerBase
{
    private readonly SyspharmaContext _context;
    public ConfiguracionController(SyspharmaContext context) => _context = context;

    [HttpGet("{clave}")]
    public async Task<IActionResult> Get(string clave)
    {
        var config = await _context.Configuraciones
            .FirstOrDefaultAsync(c => c.Clave == clave);
        if (config == null) return NotFound();
        return Ok(new { config.Clave, config.Valor, config.Descripcion });
    }

    [HttpPut("{clave}")]
    public async Task<IActionResult> Update(string clave, [FromBody] string valor)
    {
        var config = await _context.Configuraciones
            .FirstOrDefaultAsync(c => c.Clave == clave);
        if (config == null) return NotFound();
        config.Valor = valor;
        config.FechaActualizacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(new { config.Clave, config.Valor });
    }
}