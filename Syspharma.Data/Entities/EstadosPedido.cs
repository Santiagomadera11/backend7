using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class EstadosPedido
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
