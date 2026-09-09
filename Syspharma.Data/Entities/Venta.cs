using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Venta
{
    public int Id { get; set; }
    public string NumeroVenta { get; set; } = null!;
    public int TurnoId { get; set; }
    public int UsuarioId { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteDocumento { get; set; }
    public string? ClienteTelefono { get; set; }
    public int MetodoPagoId { get; set; }
    public int EstadoId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal PorcentajeIva { get; set; }
    public decimal Total { get; set; }
    public string? Notas { get; set; }
    public DateTime? FechaVenta { get; set; }
    public string? Origen { get; set; } = "CAJA";
    public int? PedidoId { get; set; }
    public string? ReferenciasPago { get; set; }

    public virtual EstadosVentum Estado { get; set; } = null!;
    public virtual MetodosPago MetodoPago { get; set; } = null!;
    public virtual Turno Turno { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Pedido? Pedido { get; set; }

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    // Relación con los servicios agregada
    public virtual ICollection<VentaDetalleServicio> VentaDetallesServicios { get; set; } = new List<VentaDetalleServicio>();
}