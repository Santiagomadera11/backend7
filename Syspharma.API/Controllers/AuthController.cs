using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.API.Services;
using Syspharma.Domain.DTOs;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SyspharmaContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<Usuario> _userManager;

        public AuthController(SyspharmaContext context, IConfiguration config, ILogger<AuthController> logger, IMemoryCache cache, IEmailSender emailSender, UserManager<Usuario> userManager) =>
            (_context, _config, _logger, _cache, _emailSender, _userManager) = (context, config, logger, cache, emailSender, userManager);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolesPermisos)
                        .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Estado == true);

            if (usuario == null)
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo" });

            var passwordValido = await _userManager.CheckPasswordAsync(usuario, dto.Password);
            if (!passwordValido)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var permisos = usuario.Role?.RolesPermisos
                .Select(rp => rp.Permiso.Codigo)
                .ToList() ?? new List<string>();

            var token = GenerarToken(usuario);
            var expiresInMinutes = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var m) ? m : 60;

            return Ok(new
            {
                token,
                expiresInMinutes,
                user = new
                {
                    id = usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    rol = usuario.Role?.Nombre ?? "Sin Rol",
                    rolId = usuario.RoleId,
                    usuario.Avatar,
                    usuario.Estado,
                    usuario.Documento,
                    usuario.Telefono,
                    usuario.Direccion,
                    usuario.TipoDocumentoId,
                    permisos
                }
            });
        }

        // Autoedición de perfil: solo el propio usuario autenticado puede editar sus datos
        // (editar A OTROS usuarios es responsabilidad de UsuarioController, que exige el
        // permiso users.edit). Antes esta ruta no tenía [Authorize] ni verificaba el
        // dueño del token, así que cualquiera (sin sesión) podía reescribir el perfil de
        // cualquier usuario por ID.
        //
        // Usa su propio DTO (UpdateMiPerfilDto) en vez del UsuarioUpdateDto que usa el
        // admin: ese último exige RolId/Estado como obligatorios (los necesita el admin
        // para editar a otros), pero el front de "Mi Perfil" nunca los manda. Como
        // AuthController tiene [ApiController], el ModelState inválido corta la petición
        // con un 400 automático ANTES de que el código del método llegue a ejecutarse, así
        // que un ModelState.Remove("RolId") acá adentro nunca alcanza a correr.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateMiPerfilDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var loggedInUserId))
                return Unauthorized();

            if (loggedInUserId != id)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "No puedes editar el perfil de otro usuario." });

            if (id != dto.Id)
                return BadRequest(new { message = "El ID del usuario no coincide con la URL" });

            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (usuario.Email != dto.Email)
            {
                var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != id);
                if (emailExiste)
                    return BadRequest(new { message = "El correo electrónico ya está en uso por otro usuario" });

                usuario.Email = dto.Email;
                usuario.UserName = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.Documento))
            {
                var documentoExiste = await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento && u.Id != id);
                if (documentoExiste)
                    return BadRequest(new { message = "El documento ya se encuentra registrado" });
            }

            usuario.Nombre = dto.Nombre.Trim();
            usuario.TipoDocumentoId = dto.TipoDocumentoId;
            usuario.Documento = string.IsNullOrEmpty(dto.Documento) ? null : dto.Documento;
            usuario.Telefono = string.IsNullOrEmpty(dto.Telefono) ? null : dto.Telefono;
            usuario.Direccion = string.IsNullOrEmpty(dto.Direccion) ? null : dto.Direccion;

            var resultadoUpdate = await _userManager.UpdateAsync(usuario);
            if (!resultadoUpdate.Succeeded)
                return BadRequest(new { message = "Error al actualizar el perfil", errors = resultadoUpdate.Errors });

            return Ok(new
            {
                message = "Perfil actualizado correctamente",
                user = new
                {
                    id = usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.TipoDocumentoId,
                    usuario.Documento,
                    usuario.Telefono
                }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email requerido" });

            var userEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Estado == true);
            if (userEntity == null)
                return BadRequest(new { message = "No existe un usuario con ese email" });

            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            _cache.Set($"recovery_{dto.Email}", code, TimeSpan.FromMinutes(15));

            // ── Template mejorado de recuperación ──────────────────────────
            var htmlRecuperacion = EmailTemplates.RecuperacionContrasena(userEntity.Nombre ?? "Usuario", code);

            try
            {
                await _emailSender.SendEmailAsync(userEntity.Email!, "Código de recuperación — SysPharma", htmlRecuperacion);
                return Ok(new { message = "Código enviado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo a {Email}", userEntity.Email);
                return BadRequest($"Error enviando correo: {ex.Message}");
            }
        }

        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeDto dto)
        {
            // Solo confirma para la UI que el código es correcto — a propósito NO lo
            // borra del caché acá. El único paso que puede consumirlo es ResetPassword,
            // que es el que de verdad cambia la contraseña.
            if (!_cache.TryGetValue($"recovery_{dto.Email}", out string? codeGuardado))
                return BadRequest(new { message = "Código expirado o no encontrado." });

            if (codeGuardado?.Trim() != dto.Code.Trim())
                return BadRequest(new { message = "Código incorrecto." });

            return Ok(new { message = "Código verificado correctamente." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { message = "Email y nueva contraseña son requeridos." });

            // Antes este endpoint reseteaba la contraseña con solo el email, sin pedir
            // el código de verificación — cualquiera podía tomar cualquier cuenta con
            // solo conocer su correo. Ahora exige el mismo código enviado por mail y lo
            // valida acá, que es el único lugar donde efectivamente se consume.
            if (string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest(new { message = "El código de verificación es requerido." });

            if (!_cache.TryGetValue($"recovery_{dto.Email}", out string? codeGuardado))
                return BadRequest(new { message = "Código expirado o no encontrado. Solicita uno nuevo." });

            if (codeGuardado?.Trim() != dto.Code.Trim())
                return BadRequest(new { message = "Código incorrecto." });

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest(new { message = "Usuario no encontrado." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resultado = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!resultado.Succeeded)
                return BadRequest(new { message = "Error al cambiar la contraseña.", errors = resultado.Errors });

            _cache.Remove($"recovery_{dto.Email}");
            return Ok(new { message = "Contraseña actualizada correctamente." });
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email!),
                new Claim(ClaimTypes.Role, usuario.Role?.Nombre ?? "Sin Rol"),
                new Claim("nombre", usuario.Nombre ?? "")
            };

            var keyStr = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyStr)) throw new Exception("JWT Key no configurada en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }


    public class UpdateMiPerfilDto
    {
        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string Email { get; set; } = null!;

        [StringLength(20, ErrorMessage = "El número de documento no puede superar los 20 caracteres.")]
        public string? Documento { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El tipo de documento seleccionado no es válido.")]
        public int? TipoDocumentoId { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        public string? Direccion { get; set; }
    }

    public class ForgotPasswordDto { public string Email { get; set; } = null!; }
    public class VerifyCodeDto { public string Email { get; set; } = null!; public string Code { get; set; } = null!; }
    public class ResetPasswordDto { public string Email { get; set; } = null!; public string Code { get; set; } = null!; public string NewPassword { get; set; } = null!; }
}