using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class CategoriaServicio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
