namespace Syspharma.Domain.DTOs
{
    public class MedicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Especialidad { get; set; }
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? DiasLaborales { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFin { get; set; }
        public int? Intervalo { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }

    public class MedicoCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string? Especialidad { get; set; }
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }

    public class MedicoUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Especialidad { get; set; }
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }

    public class HorarioDiaDto
    {
        public byte DiaSemana { get; set; }
        public string? MananaInicio { get; set; }
        public string? MananaFin { get; set; }
        public string? TardeInicio { get; set; }
        public string? TardeFin { get; set; }
    }

    public class GuardarHorarioDto
    {
        public int MedicoId { get; set; }
        public List<HorarioDiaDto> Horarios { get; set; } = new();
    }

    public class DiaNoDisponibleCreateDto
    {
        public int MedicoId { get; set; }
        public string FechaInicio { get; set; } = null!;
        public string FechaFin { get; set; } = null!;
        public string? Motivo { get; set; }
    }

    public class DiaNoDisponibleDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string FechaInicio { get; set; } = null!;
        public string FechaFin { get; set; } = null!;
        public string? Motivo { get; set; }
    }
}