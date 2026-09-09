using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class UsuarioCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El tipo de documento seleccionado no es válido.")]
        public int? TipoDocumentoId { get; set; }

        [StringLength(20, ErrorMessage = "El número de documento no puede superar los 20 caracteres.")]
        public string? Documento { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El rol seleccionado no es válido.")]
        public int RolId { get; set; }

        public bool Estado { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string Contrasena { get; set; } = null!;
    }
}