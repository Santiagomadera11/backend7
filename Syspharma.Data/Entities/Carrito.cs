using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Carrito
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<CarritoItem> Items { get; set; } = new List<CarritoItem>();
}
