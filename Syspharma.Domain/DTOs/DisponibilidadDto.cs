using System;
using System.Collections.Generic;

namespace Syspharma.Domain.DTOs
{
    public class HorarioDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public int DiaSemana { get; set; }
        public string MananaInicio { get; set; } = null!;
        public string MananaFin { get; set; } = null!;
        public string TardeInicio { get; set; } = null!;
        public string TardeFin { get; set; } = null!;
    }

    public class HorarioItemDto
    {
        public int DiaSemana { get; set; }
        public string MananaInicio { get; set; } = null!;
        public string MananaFin { get; set; } = null!;
        public string TardeInicio { get; set; } = null!;
        public string TardeFin { get; set; } = null!;
    }

    public class GuardarHorarioItemDto
    {
        public int MedicoId { get; set; }
        public List<HorarioItemDto> Horarios { get; set; } = new();
    }

    public class BloqueoDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public string? Motivo { get; set; }
    }

    public class BloqueoCreateDto
    {
        public int MedicoId { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public string? Motivo { get; set; }
    }
}
