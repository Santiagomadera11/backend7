using System;

namespace Syspharma.Data.Entities;

public partial class VentaDetalleLote
{
    public int Id { get; set; }

    public int VentaDetalleId { get; set; }

    public int LoteId { get; set; }

    public int Cantidad { get; set; }

    public virtual VentaDetalle VentaDetalle { get; set; } = null!;

    public virtual Lote Lote { get; set; } = null!;
}
