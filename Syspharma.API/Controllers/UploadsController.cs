using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syspharma.API.Services;
using System.Linq;

namespace Syspharma.API.Controllers
{
    // Endpoint genérico de subida de imágenes a Cloudinary. Lo usan pantallas que
    // suben una imagen suelta antes de que exista el recurso al que pertenece
    // (ej: la foto de un producto se elige en el formulario de creación, antes de
    // que el producto tenga Id) — se sube primero y se guarda la URL resultante
    // junto con el resto del formulario.
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

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(archivo.FileName).ToLower();
            if (!extensionesPermitidas.Contains(extension))
                return BadRequest(new { message = "Solo se permiten JPG, PNG o WEBP" });

            // La carpeta la elige el llamador (ej: "productos"), pero siempre queda
            // bajo el namespace del proyecto para no mezclar assets de otras apps
            // que compartan la misma cuenta de Cloudinary.
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
