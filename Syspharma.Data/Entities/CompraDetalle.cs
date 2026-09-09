using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class CompraDetalle
{
    public int Id { get; set; }

    public int CompraId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public string? Lote { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public virtual Compra Compra { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
