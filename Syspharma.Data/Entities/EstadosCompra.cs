using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class EstadosCompra
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
