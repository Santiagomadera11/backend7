using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class ProductoFormaVenta
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string Tipo { get; set; } = null!;   // "Unidad" | "Blister" | "Caja"
    public decimal Precio { get; set; }
    public int FactorUnidades { get; set; }
    public bool Activo { get; set; } = true;

    public virtual Producto Producto { get; set; } = null!;
}
