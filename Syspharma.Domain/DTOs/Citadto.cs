using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    // DTO para los selectores de estado
    public class CitaEstadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class CitaDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string MedicoNombre { get; set; } = null!;
        public string PacienteNombre { get; set; } = null!;
        public string? PacienteDocumento { get; set; }
        public string? PacienteTelefono { get; set; }
        public string? PacienteEmail { get; set; }
        public int? ServicioId { get; set; }
        public string? ServicioNombre { get; set; }
        public decimal? Precio { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public int? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public int? PedidoId { get; set; }
        public int? VentaId { get; set; }
        public string Fecha { get; set; } = null!;
        public string Hora { get; set; } = null!;
        public string? Notas { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }

    public class CitaCreateDto
    {
        [Required(ErrorMessage = "El médico es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El médico seleccionado no es válido.")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "El nombre del paciente es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre del paciente no puede superar los 150 caracteres.")]
        public string PacienteNombre { get; set; } = null!;

        [StringLength(20, ErrorMessage = "El documento del paciente no puede superar los 20 caracteres.")]
        public string? PacienteDocumento { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? PacienteTelefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string? PacienteEmail { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El servicio seleccionado no es válido.")]
        public int? ServicioId { get; set; }

        public int? UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria.")]
        public string Fecha { get; set; } = null!;

        [Required(ErrorMessage = "La hora de la cita es obligatoria.")]
        public string Hora { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }
    }

    public class CitaUpdateDto
    {
        [Required(ErrorMessage = "El ID de la cita es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El médico seleccionado no es válido.")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "El nombre del paciente es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre del paciente no puede superar los 150 caracteres.")]
        public string PacienteNombre { get; set; } = null!;

        [StringLength(20, ErrorMessage = "El documento del paciente no puede superar los 20 caracteres.")]
        public string? PacienteDocumento { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? PacienteTelefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string? PacienteEmail { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El servicio seleccionado no es válido.")]
        public int? ServicioId { get; set; }

        [Required(ErrorMessage = "El estado de la cita es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El estado seleccionado no es válido.")]
        public int EstadoId { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatoria.")]
        public string Fecha { get; set; } = null!;

        [Required(ErrorMessage = "La hora de la cita es obligatoria.")]
        public string Hora { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }
    }
}