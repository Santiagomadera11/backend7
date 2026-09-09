using System;

namespace Syspharma.Data.Entities;
public class Configuracion
{
    public int Id { get; set; }
    public string Clave { get; set; } = null!;
    public string Valor { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}