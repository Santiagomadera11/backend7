using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Syspharma.API.Services
{
    public interface IImageUploadService
    {
        Task<string> SubirImagen(IFormFile archivo, string carpeta);
    }

    // Sube imágenes a Cloudinary en vez de al disco local: el disco de un servicio web
    // típico (Render, etc.) es efímero y se borra en cada redeploy/reinicio, así que
    // cualquier foto guardada ahí desaparece tarde o temprano. Cloudinary la sirve desde
    // su propio CDN, independiente del ciclo de vida del backend.
    public class CloudinaryImageUploadService : IImageUploadService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageUploadService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
                throw new InvalidOperationException("Faltan credenciales de Cloudinary en la configuración (Cloudinary:CloudName/ApiKey/ApiSecret).");

            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        }

        public async Task<string> SubirImagen(IFormFile archivo, string carpeta)
        {
            await using var stream = archivo.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(archivo.FileName, stream),
                Folder = carpeta,
            };

            var resultado = await _cloudinary.UploadAsync(uploadParams);
            if (resultado.Error != null)
                throw new Exception($"Error al subir la imagen a Cloudinary: {resultado.Error.Message}");

            return resultado.SecureUrl.ToString();
        }
    }
}
