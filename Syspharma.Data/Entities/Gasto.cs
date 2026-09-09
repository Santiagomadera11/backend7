using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Gasto
{
    public int Id { get; set; }
    public int TurnoId { get; set; }
    public int UsuarioId { get; set; }
    public string Concepto { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Monto { get; set; }
    public string Categoria { get; set; } = null!;
    public string? Comprobante { get; set; }
    public DateTime? FechaGasto { get; set; }

    public bool Anulado { get; set; }

    public DateTime? FechaAnulacion { get; set; }

    public string? MotivoAnulacion { get; set; }

    public virtual Turno Turno { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}