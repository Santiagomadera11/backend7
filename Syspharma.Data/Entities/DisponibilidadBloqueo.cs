using System;

namespace Syspharma.Data.Entities;

public partial class DisponibilidadBloqueo
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string? Motivo { get; set; }

    public virtual Medico Medico { get; set; } = null!;
}
