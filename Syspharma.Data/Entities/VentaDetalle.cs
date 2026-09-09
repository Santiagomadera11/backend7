using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class VentaDetalle
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Descuento { get; set; }

    public decimal Subtotal { get; set; }

    public int? LoteId { get; set; }

    public virtual Lote? Lote { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
