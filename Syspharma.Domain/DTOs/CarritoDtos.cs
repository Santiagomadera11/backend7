using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class CarritoItemDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
        public int Cantidad { get; set; }
        public int Stock { get; set; }
    }

    public class CarritoItemUpsertDto
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}
