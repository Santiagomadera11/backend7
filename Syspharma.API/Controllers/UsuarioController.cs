using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syspharma.Business.Services;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Syspharma.API.Filters;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly UserManager<Usuario> _userManager;
        private readonly SyspharmaContext _context;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService service, UserManager<Usuario> userManager, SyspharmaContext context, ILogger<UsuarioController> logger)
        {
            _service = service;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [RequirePermission("users.view")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _service.ObtenerTodos();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var loggedInUserId))
            {
                return Unauthorized();
            }

            // Permitir si es el mismo usuario logueado
            if (loggedInUserId == id)
            {
                var selfUsuario = await _service.ObtenerPorId(id);
                if (selfUsuario == null) return NotFound(new { message = "Usuario no encontrado" });
                return Ok(selfUsuario);
            }

            // Si es otro usuario, verificar si tiene el permiso "users.view"
            var usuarioLogueado = await _context.Usuarios
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolesPermisos)
                        .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(u => u.Id == loggedInUserId);

            var esAdmin = usuarioLogueado?.Role?.Nombre?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ?? false;
            var tienePermiso = esAdmin || (usuarioLogueado?.Role?.RolesPermisos?.Any(rp => 
                rp.Permiso.Codigo.Equals("users.view", StringComparison.OrdinalIgnoreCase)) ?? false);

            if (!tienePermiso)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tienes el permiso requerido: users.view" });
            }

            var usuario = await _service.ObtenerPorId(id);
            if (usuario == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(usuario);
        }

        [HttpPost]
        [RequirePermission("users.create")]
        public async Task<IActionResult> Crear([FromBody] UsuarioCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Contrasena))
                    return BadRequest(new { message = "La contraseña es obligatoria" });

                if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                    return BadRequest(new { message = "El correo ya está registrado" });

                if (!string.IsNullOrEmpty(dto.Documento) && await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento))
                    return BadRequest(new { message = "El número de documento ya está registrado por otro usuario." });

                var usuario = new Usuario
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Nombre = dto.Nombre,
                    Documento = dto.Documento,
                    TipoDocumentoId = dto.TipoDocumentoId,
                    Telefono = dto.Telefono,
                    RoleId = dto.RolId,
                    FechaCreacion = DateTime.Now,
                    Estado = dto.Estado,
                };

                var resultado = await _userManager.CreateAsync(usuario, dto.Contrasena);
                if (!resultado.Succeeded)
                    return BadRequest(new { message = string.Join(", ", resultado.Errors.Select(e => e.Description)) });

                await _context.Entry(usuario).Reference(u => u.Role).LoadAsync();
                await _context.Entry(usuario).Reference(u => u.TipoDocumento).LoadAsync();

                return Ok(new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Documento = usuario.Documento,
                    TipoDocumento = usuario.TipoDocumento?.Nombre,
                    TipoDocumentoId = usuario.TipoDocumentoId,
                    Telefono = usuario.Telefono,
                    RolNombre = usuario.Role?.Nombre ?? "",
                    Avatar = usuario.Avatar,
                    Estado = usuario.Estado,
                    RolId = usuario.RoleId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [RequirePermission("users.edit")]
        public async Task<IActionResult> Actualizar([FromBody] UsuarioUpdateDto dto)
        {
            try
            {
                var usuario = await _service.Actualizar(dto);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/foto")]
        [Consumes("multipart/form-data")]
        [RequirePermission("users.edit")]
        public async Task<IActionResult> SubirFoto(int id, IFormFile foto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null) return NotFound(new { message = "Usuario no encontrado" });

                if (foto == null || foto.Length == 0)
                    return BadRequest(new { message = "No se envió ninguna imagen" });

                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(foto.FileName).ToLower();
                if (!extensionesPermitidas.Contains(extension))
                    return BadRequest(new { message = "Solo se permiten JPG, PNG o WEBP" });

                // Eliminar foto anterior
                if (!string.IsNullOrEmpty(usuario.Avatar))
                {
                    var fotoAnterior = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        usuario.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(fotoAnterior))
                        System.IO.File.Delete(fotoAnterior);
                }

                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos-perfil");
                Directory.CreateDirectory(carpeta);

                var nombreArchivo = $"user_{id}_{DateTime.Now.Ticks}{extension}";
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    await foto.CopyToAsync(stream);

                usuario.Avatar = $"/fotos-perfil/{nombreArchivo}";
                await _context.SaveChangesAsync();

                return Ok(new { avatar = usuario.Avatar });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        [RequirePermission("users.status")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool estado)
        {
            try
            {
                var usuario = await _service.CambiarEstado(id, estado);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [RequirePermission("users.delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.Eliminar(id);
                if (!ok) return NotFound(new { message = "Usuario no encontrado" });
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}: {Inner}", id, ex.InnerException?.Message);
                return Conflict(new
                {
                    message = "No se puede eliminar el usuario por restricciones referenciales.",
                    detail = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar DELETE /api/Usuario/{UserId}", id);
                return BadRequest(new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }
    }
}