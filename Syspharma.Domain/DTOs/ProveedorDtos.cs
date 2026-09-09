using System;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class ProveedorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Contacto { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public int? TipoDocumentoId { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public int? EstadoId { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }

    public class ProveedorCreateDto
    {
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre del proveedor no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(100, ErrorMessage = "El nombre de contacto no puede superar los 100 caracteres.")]
        public string? Contacto { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        public string? Direccion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El tipo de documento seleccionado no es válido.")]
        public int? TipoDocumentoId { get; set; }

        [StringLength(20, ErrorMessage = "El número de documento no puede superar los 20 caracteres.")]
        public string? Documento { get; set; }
    }

    public class ProveedorUpdateDto
    {
        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre del proveedor no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(100, ErrorMessage = "El nombre de contacto no puede superar los 100 caracteres.")]
        public string? Contacto { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        public string? Direccion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El tipo de documento seleccionado no es válido.")]
        public int? TipoDocumentoId { get; set; }

        [StringLength(20, ErrorMessage = "El número de documento no puede superar los 20 caracteres.")]
        public string? Documento { get; set; }
    }
}