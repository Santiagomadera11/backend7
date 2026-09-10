using System;

namespace Syspharma.Data.Entities;

public partial class Notificacion
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Tipo { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string? Mensaje { get; set; }

    public string? Path { get; set; }

    public bool Leida { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
