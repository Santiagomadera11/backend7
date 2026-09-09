using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Syspharma.Data.Entities;

public partial class Usuario : IdentityUser<int>
{
    public string Nombre { get; set; } = null!;

    public int? TipoDocumentoId { get; set; }

    public string? Documento { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public int RoleId { get; set; }

    public string? Avatar { get; set; }

    public bool Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual Role Role { get; set; } = null!;

    public virtual TiposDocumento? TipoDocumento { get; set; }

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();


}