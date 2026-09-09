using System;

namespace Syspharma.Data.Entities;

public partial class Lote
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public int? CompraId { get; set; }

    public string NumeroLote { get; set; } = null!;

    public int Cantidad { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal CostoUnitario { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public virtual Producto Producto { get; set; } = null!;

    public virtual Compra? Compra { get; set; }
}
