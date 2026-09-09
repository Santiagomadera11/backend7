using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Register payload: {@dto}", dto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "Nombre, email y password son obligatorios" });

            if (!await _context.Database.CanConnectAsync())
                return StatusCode(503, new { message = "No se puede conectar a la base de datos." });

            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "El email ya está registrado" });

            if (!string.IsNullOrEmpty(dto.Documento) && await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento))
                return BadRequest(new { message = "El documento ya está registrado" });

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                UserName = dto.Email,
                RoleId = dto.RoleId,
                Documento = string.IsNullOrEmpty(dto.Documento) ? null : dto.Documento,
                TipoDocumentoId = dto.TipoDocumentoId,
                Telefono = dto.Telefono,
                Estado = true,
                FechaCreacion = DateTime.Now
            };

            var resultado = await _userManager.CreateAsync(usuario, dto.Password);
            if (!resultado.Succeeded)
                return BadRequest(resultado.Errors);

            // ── Correo de bienvenida ──────────────────────────────────────
            var frontendUrl = _config["EmailSettings:FrontendUrl"] ?? "http://localhost:5173";
            try
            {
                // Caso A: cliente se registra solo (no trae contraseña temporal)
                if (string.IsNullOrEmpty(dto.PasswordTemporal))
                {
                    var htmlBienvenida = EmailTemplates.Bienvenida(usuario.Nombre, usuario.Email!, frontendUrl);
                    await _emailSender.SendEmailAsync(usuario.Email!, "¡Bienvenido a SysPharma! 🎉", htmlBienvenida);
                }
                // Caso B: admin crea usuario con contraseña temporal
                else
                {
                    var htmlCredenciales = EmailTemplates.BienvenidaConCredenciales(usuario.Nombre, usuario.Email!, dto.PasswordTemporal, frontendUrl);
                    await _emailSender.SendEmailAsync(usuario.Email!, "Tu cuenta en SysPharma está lista 🎉", htmlCredenciales);
                }
            }
            catch (Exception ex)
            {
                // No bloqueamos el registro si el correo falla — solo lo logueamos
                _logger.LogWarning(ex, "No se pudo enviar correo de bienvenida a {Email}", usuario.Email);
            }

            return Ok(new { message = "Usuario registrado correctamente" });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] Syspharma.Domain.DTOs.UsuarioUpdateDto dto)
        {
            ModelState.Remove("RoId");
            ModelState.Remove("RolId");
            ModelState.Remove("Estado");

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos en el formulario", errors = ModelState });

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

            usuario.Nombre = $"{dto.Nombre} {dto.Apellidos}".Trim();
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
            if (!_cache.TryGetValue($"recovery_{dto.Email}", out string? codeGuardado))
                return BadRequest(new { message = "Código expirado o no encontrado." });

            if (codeGuardado?.Trim() != dto.Code.Trim())
                return BadRequest(new { message = "Código incorrecto." });

            _cache.Remove($"recovery_{dto.Email}");
            return Ok(new { message = "Código verificado correctamente." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { message = "Email y nueva contraseña son requeridos." });

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest(new { message = "Usuario no encontrado." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resultado = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!resultado.Succeeded)
                return BadRequest(new { message = "Error al cambiar la contraseña.", errors = resultado.Errors });

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

    public class RegisterDto
    {
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public string? Documento { get; set; }
        public int? TipoDocumentoId { get; set; }
        public string? Telefono { get; set; }
        // Opcional — solo se envía cuando el admin crea el usuario
        public string? PasswordTemporal { get; set; }
    }

    public class ForgotPasswordDto { public string Email { get; set; } = null!; }
    public class VerifyCodeDto { public string Email { get; set; } = null!; public string Code { get; set; } = null!; }
    public class ResetPasswordDto { public string Email { get; set; } = null!; public string NewPassword { get; set; } = null!; }
}