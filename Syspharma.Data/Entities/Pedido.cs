using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Pedido
{
    public int Id { get; set; }

    public string NumeroPedido { get; set; } = null!;

    public int? UsuarioId { get; set; }

    public string ClienteNombre { get; set; } = null!;

    public string? ClienteDocumento { get; set; }

    public string? ClienteTelefono { get; set; }

    public string? ClienteEmail { get; set; }

    public string? Direccion { get; set; }

    public int? MetodoPagoId { get; set; }

    public int EstadoId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public string? Notas { get; set; }

    public string? Origen { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public virtual EstadosPedido Estado { get; set; } = null!;

    public virtual MetodosPago? MetodoPago { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();

    public virtual Usuario? Usuario { get; set; }
}
