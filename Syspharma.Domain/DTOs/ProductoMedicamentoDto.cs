namespace Syspharma.Domain.DTOs
{
    public class ProductoMedicamentoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? Composicion { get; set; }
        public string? Concentracion { get; set; }
        public string? ViaAdministracion { get; set; }
        public string? RegistroSanitario { get; set; }
        public bool? RequiereFormula { get; set; }

    }
}
