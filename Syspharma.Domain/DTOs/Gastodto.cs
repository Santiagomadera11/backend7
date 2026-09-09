using System;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class GastoDto
    {
        public int Id { get; set; }
        public int TurnoId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        public string? NumeroGasto { get; set; }
        public string Concepto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Monto { get; set; }
        public string Categoria { get; set; } = null!;
        public int? MetodoPagoId { get; set; }
        public string? MetodoPago { get; set; }
        public int? EstadoId { get; set; }
        public string? Estado { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Iva { get; set; }
        public decimal? PorcentajeIva { get; set; }
        public string? Notas { get; set; }
        public string? Proveedor { get; set; }
        public string? Comprobante { get; set; }
        public string? ComprobanteUrl { get; set; }
        public DateTime? FechaGasto { get; set; }
        public DateTime? FechaCreacion { get; set; }

        // Campos específicos para el frontend (ExpensesModal)
        public string? Hora { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Observaciones { get; set; }
        public bool? Anulado { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string? MotivoAnulacion { get; set; }
    }

    public class GastoCreateDto
    {
        [Required(ErrorMessage = "El ID del turno es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de turno seleccionado no es válido.")]
        public int TurnoId { get; set; }

        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario seleccionado no es válido.")]
        public int UsuarioId { get; set; }

        [StringLength(50, ErrorMessage = "El número de gasto no puede superar los 50 caracteres.")]
        public string? NumeroGasto { get; set; }

        [Required(ErrorMessage = "El concepto del gasto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El concepto no puede superar los 150 caracteres.")]
        public string Concepto { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El monto del gasto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto del gasto debe ser mayor que cero.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La categoría del gasto es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede superar los 100 caracteres.")]
        public string Categoria { get; set; } = "operacional";

        [Range(1, int.MaxValue, ErrorMessage = "El método de pago seleccionado no es válido.")]
        public int? MetodoPagoId { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        public string? Comprobante { get; set; }

        public DateTime? FechaGasto { get; set; }
    }

    public class GastoUpdateDto
    {
        [Required(ErrorMessage = "El ID del gasto es obligatorio.")]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "El número de gasto no puede superar los 50 caracteres.")]
        public string? NumeroGasto { get; set; }

        [Required(ErrorMessage = "El concepto del gasto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El concepto no puede superar los 150 caracteres.")]
        public string Concepto { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El monto del gasto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto del gasto debe ser mayor que cero.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La categoría del gasto es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede superar los 100 caracteres.")]
        public string Categoria { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "El método de pago seleccionado no es válido.")]
        public int? MetodoPagoId { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        public string? Comprobante { get; set; }

        public DateTime? FechaGasto { get; set; }
    }

    // NUEVO: DTO para KPIs
    public class GastoKpiDto
    {
        public decimal TotalGastosDia { get; set; }
        public int CantidadGastosDia { get; set; }
        public decimal TotalNomina { get; set; }
        public decimal TotalServicios { get; set; }
        public decimal TotalMantenimiento { get; set; }
    }
}
