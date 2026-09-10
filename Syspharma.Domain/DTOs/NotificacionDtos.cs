using System;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public string? Mensaje { get; set; }
        public string? Path { get; set; }
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class NotificacionCreateDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(30)]
        public string Tipo { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = null!;

        public string? Mensaje { get; set; }

        public string? Path { get; set; }
    }
}
