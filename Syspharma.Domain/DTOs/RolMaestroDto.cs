using System;
using System.Collections.Generic;

namespace Syspharma.Domain.DTOs
{
    public class RolMaestroDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? FechaCreacion { get; set; }
        public List<string> Permisos { get; set; } = new();
    }

    public class RolMaestroEstadoDto
    {
        public bool Estado { get; set; }
    }

    public class PermisoListaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Categoria { get; set; } = null!;
    }
}