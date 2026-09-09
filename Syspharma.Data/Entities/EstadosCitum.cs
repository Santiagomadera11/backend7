using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class EstadosCitum
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();
}
