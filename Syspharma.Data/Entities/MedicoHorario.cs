using System.Collections.Generic;
namespace Syspharma.Data.Entities;

public class MedicoHorario
{
	public int Id { get; set; }
	public int MedicoId { get; set; }
	public byte DiaSemana { get; set; } // 0=Dom..6=Sab
	public TimeOnly? MananaInicio { get; set; }
	public TimeOnly? MananaFin { get; set; }
	public TimeOnly? TardeInicio { get; set; }
	public TimeOnly? TardeFin { get; set; }

	public virtual Medico Medico { get; set; } = null!;
}