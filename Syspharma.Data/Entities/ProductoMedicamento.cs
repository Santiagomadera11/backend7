using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class ProductoMedicamento
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public string? Composicion { get; set; }

    public string? Concentracion { get; set; }

    public string? ViaAdministracion { get; set; }

    public string? RegistroSanitario { get; set; }

    public bool? RequiereFormula { get; set; }

    // --- NUEVA RELACIÓN INVERSA ---
    public virtual Producto Producto { get; set; } = null!;
}
