using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class PresentacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int ProductosCount { get; set; }
    }

    public class PresentacionCreateDto
    {
        [Required(ErrorMessage = "El nombre de la presentación es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }
    }

    public class PresentacionUpdateDto
    {
        [Required(ErrorMessage = "El ID de la presentación es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la presentación es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }
    }
}
