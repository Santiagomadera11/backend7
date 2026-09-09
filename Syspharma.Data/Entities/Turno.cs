using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Turno
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Estado { get; set; } = null!;

    public decimal MontoBase { get; set; }

    public decimal? MontoFinal { get; set; }

    public decimal TotalVentas { get; set; }

    public decimal TotalGastos { get; set; }

    public int ResumenVentas { get; set; }

    public int ResumenServicios { get; set; }

    public decimal ResumenErroresCaja { get; set; }

    public decimal? Diferencia { get; set; }

    public string? Notas { get; set; }

    public DateTime? FechaApertura { get; set; }

    public DateTime? FechaCierre { get; set; }

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
