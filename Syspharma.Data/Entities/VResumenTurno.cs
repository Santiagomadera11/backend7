using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class VResumenTurno
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;

    public decimal MontoBase { get; set; }

    public DateTime? FechaApertura { get; set; }

    public DateTime? FechaCierre { get; set; }

    public string Empleado { get; set; } = null!;

    public decimal TotalVentas { get; set; }

    public decimal TotalGastos { get; set; }
}
