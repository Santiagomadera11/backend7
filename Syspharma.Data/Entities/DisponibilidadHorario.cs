namespace Syspharma.Data.Entities;

public partial class DisponibilidadHorario
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public int DiaSemana { get; set; }
    public string MananaInicio { get; set; } = null!;
    public string MananaFin { get; set; } = null!;
    public string TardeInicio { get; set; } = null!;
    public string TardeFin { get; set; } = null!;

    public virtual Medico Medico { get; set; } = null!;
}
