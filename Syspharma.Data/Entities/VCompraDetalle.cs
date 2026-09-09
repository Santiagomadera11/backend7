using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class VCompraDetalle
{
    public int Id { get; set; }

    public int CompraId { get; set; }

    public int ProductoId { get; set; }

    public string ProductoNombre { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? SubtotalCalculado { get; set; }
}
