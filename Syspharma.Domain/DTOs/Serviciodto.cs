namespace Syspharma.Domain.DTOs
{
    public class ServicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }

    public class ServicioCreateDto
    {
        public string Nombre { get; set; } = null!;
        public int CategoriaId { get; set; }
        public decimal Precio { get; set; }
        public int? Duracion { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ServicioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int CategoriaId { get; set; }
        public decimal Precio { get; set; }
        public int? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}