namespace Syspharma.Domain.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Documento { get; set; }
        public string? TipoDocumento { get; set; }
        public int? TipoDocumentoId { get; set; }
        public string? Telefono { get; set; }
        public string RolNombre { get; set; } = null!;
        public string? Avatar { get; set; }
        public bool Estado { get; set; }
        public int RolId { get; set; }
    }
}