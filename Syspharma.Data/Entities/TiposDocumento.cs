using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class TiposDocumento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Proveedore> Proveedores { get; set; } = new List<Proveedore>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
