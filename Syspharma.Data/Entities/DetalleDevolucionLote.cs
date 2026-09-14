namespace Syspharma.Data.Entities;

public partial class DetalleDevolucionLote
{
    public int Id { get; set; }

    public int DetalleDevolucionId { get; set; }

    public int LoteId { get; set; }

    public int Cantidad { get; set; }

    public virtual DetalleDevolucion DetalleDevolucion { get; set; } = null!;

    public virtual Lote Lote { get; set; } = null!;
}
