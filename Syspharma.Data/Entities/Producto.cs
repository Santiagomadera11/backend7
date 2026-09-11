using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int CategoriaId { get; set; }

    public int? ProveedorId { get; set; }

    public decimal Precio { get; set; }

    public decimal? PrecioCompra { get; set; }

    public int? MarcaId { get; set; }

    public int? PresentacionId { get; set; }

    public int Stock { get; set; }

    public string? CodigoBarras { get; set; }

    public string? Imagen { get; set; }

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public DateOnly? FechaVencimientoProxima { get; set; }

    public decimal PorcentajeIva { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual Marca? Marca { get; set; }

    public virtual Presentacion? Presentacion { get; set; }

    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();

    public virtual Proveedore? Proveedor { get; set; }

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();

    // --- NUEVA RELACIÓN HACIA MEDICAMENTO ---
    public virtual ProductoMedicamento? ProductoMedicamento { get; set; }

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    public virtual ICollection<ProductoFormaVenta> FormasVenta { get; set; } = new List<ProductoFormaVenta>();
}