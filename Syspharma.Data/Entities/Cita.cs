using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Cita
{
    public int Id { get; set; }

    public int MedicoId { get; set; }

    public string PacienteNombre { get; set; } = null!;

    public string? PacienteDocumento { get; set; }

    public string? PacienteTelefono { get; set; }

    public string? PacienteEmail { get; set; }

    public string? ServicioNombre { get; set; }

    public decimal? Precio { get; set; }

    public int EstadoId { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public string? Notas { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? ServicioId { get; set; }

    public int? UsuarioId { get; set; }

    public int? PedidoId { get; set; }
    public int? VentaId { get; set; }

    public virtual EstadosCitum Estado { get; set; } = null!;

    public virtual Medico Medico { get; set; } = null!;

    public virtual Servicio? Servicio { get; set; }

    public virtual Usuario? Usuario { get; set; }
    public virtual Pedido? Pedido { get; set; }
    public virtual Venta? Venta { get; set; }
}
