namespace Syspharma.Domain.DTOs
{
    public class TurnoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal MontoBase { get; set; }
        public decimal? MontoFinal { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalGastos { get; set; }
        public int ResumenVentas { get; set; }
        public int ResumenServicios { get; set; }
        public decimal ResumenErroresCaja { get; set; }
        public decimal? Diferencia { get; set; }
        public string? Notas { get; set; }
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
    }

    public class TurnoAbrirDto
    {
        public int UsuarioId { get; set; }
        public decimal MontoBase { get; set; }
        public string? Notas { get; set; }
    }

    public class TurnoCerrarDto
    {
        public int Id { get; set; }
        public decimal MontoFinal { get; set; }
        public string? Notas { get; set; }
    }
}