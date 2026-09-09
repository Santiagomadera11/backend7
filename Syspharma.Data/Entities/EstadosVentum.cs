using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class EstadosVentum
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
