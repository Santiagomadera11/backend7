using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.API.Services;
using System.Linq;

namespace Syspharma.API.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UploadsController : ControllerBase
    {
        private readonly IImageUploadService _imageUploadService;

        public UploadsController(IImageUploadService imageUploadService)
        {
            _imageUploadService = imageUploadService;
        }

        [HttpPost("imagen")]
        public async Task<IActionResult> SubirImagen(IFormFile archivo, [FromQuery] string carpeta = "otros")
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { message = "No se envió ninguna imagen" });

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".avif" };
            var extension = Path.GetExtension(archivo.FileName).ToLower();
            if (!extensionesPermitidas.Contains(extension))
                return BadRequest(new { message = "Solo se permiten JPG, PNG, WEBP, GIF o AVIF" });

            var carpetaSegura = string.IsNullOrWhiteSpace(carpeta) ? "otros" : carpeta;

            try
            {
                var url = await _imageUploadService.SubirImagen(archivo, $"syspharma/{carpetaSegura}");
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
