namespace Syspharma.Data.Entities;

public class EstadoDevolucion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; }
    public virtual ICollection<Devolucion> Devoluciones { get; set; } = new List<Devolucion>();
}
