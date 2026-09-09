namespace Syspharma.Data.Entities;

public class DetalleDevolucion
{
    public int Id { get; set; }
    public int DevolucionId { get; set; }
    public int DetalleVentaId { get; set; }
    public int ProductoId { get; set; }
    public int CantidadDevuelta { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal? SubtotalDevuelto { get; set; }

    public virtual Devolucion Devolucion { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
    public virtual VentaDetalle DetalleVenta { get; set; } = null!;
}
