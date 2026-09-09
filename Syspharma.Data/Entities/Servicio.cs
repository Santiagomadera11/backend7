using System;
using System.Collections.Generic;
namespace Syspharma.Data.Entities;

public partial class Servicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int CategoriaId { get; set; }
    public decimal Precio { get; set; }
    public int? Duracion { get; set; }
    public string? Descripcion { get; set; }
    public bool Estado { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public virtual CategoriaServicio Categoria { get; set; } = null!;
    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();
    public virtual ICollection<VentaDetalleServicio> VentaDetallesServicios { get; set; } = new List<VentaDetalleServicio>();
}
