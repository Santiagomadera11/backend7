using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Compra
{
    public int Id { get; set; }

    public string NumeroCompra { get; set; } = null!;

    public int ProveedorId { get; set; }

    public int UsuarioId { get; set; }

    public int EstadoId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public string? Notas { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? FechaCompra { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();

    public virtual EstadosCompra Estado { get; set; } = null!;

    public virtual Proveedore Proveedor { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
