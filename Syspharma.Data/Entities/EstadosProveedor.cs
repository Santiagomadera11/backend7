using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class EstadosProveedor
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Proveedore> Proveedores { get; set; } = new List<Proveedore>();
}
