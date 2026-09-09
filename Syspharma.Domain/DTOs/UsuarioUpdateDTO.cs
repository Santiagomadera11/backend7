using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string? Apellidos { get; set; }        // ✅ Agregado

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
        public string? Direccion { get; set; }        // ✅ Agregado

        public string? Avatar { get; set; }           // ✅ Agregado (para foto)

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El rol seleccionado no es válido.")]
        public int RolId { get; set; }

        public bool Estado { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0% y 100%.")]
        public decimal PorcentajeIva { get; set; } = 19m; // ✅ Agregado, valor por defecto 19%
    }
}