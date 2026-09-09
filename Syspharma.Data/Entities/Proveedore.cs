using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Proveedore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Contacto { get; set; }

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public int? TipoDocumentoId { get; set; }

    public string? Documento { get; set; }

    public int? EstadoId { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual EstadosProveedor? Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual TiposDocumento? TipoDocumento { get; set; }
}
