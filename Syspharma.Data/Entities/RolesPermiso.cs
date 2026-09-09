using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class RolesPermiso
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PermisoId { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public virtual Permiso Permiso { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
