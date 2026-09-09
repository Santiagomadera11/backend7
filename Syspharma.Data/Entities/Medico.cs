using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Medico
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Especialidad { get; set; }

    public string? Documento { get; set; }

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public string? DiasLaborales { get; set; }

    public TimeOnly? HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public int? Intervalo { get; set; }

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<MedicoHorario> Horarios { get; set; } = new List<MedicoHorario>();

    public virtual ICollection<MedicoDiaNoDisponible> DiasNoDisponibles { get; set; } = new List<MedicoDiaNoDisponible>();

}
