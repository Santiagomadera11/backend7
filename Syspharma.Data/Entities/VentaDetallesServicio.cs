using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class VentaDetalleServicio
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int ServicioId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal Subtotal { get; set; }
    public int? CitaId { get; set; }

    public virtual Venta Venta { get; set; } = null!;
    public virtual Servicio Servicio { get; set; } = null!;
    public virtual Cita? Cita { get; set; }
}