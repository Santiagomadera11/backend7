using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Permiso
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Categoria { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<RolesPermiso> RolesPermisos { get; set; } = new List<RolesPermiso>();
}
