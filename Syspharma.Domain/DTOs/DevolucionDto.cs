using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class DevolucionDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public string NumeroVenta { get; set; } = "";
        public string ClienteNombre { get; set; } = "";
        public string ClienteDocumento { get; set; } = "";
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = "";
        public string Motivo { get; set; } = "";
        public string? Observaciones { get; set; }
        public decimal TotalDevolucion { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public DateTime? FechaGestion { get; set; }
        public int? UsuarioGestionId { get; set; }
        public List<DetalleDevolucionDto> Detalles { get; set; } = new();
    }

    public class DetalleDevolucionDto
    {
        public int Id { get; set; }
        public int DevolucionId { get; set; }
        public int DetalleVentaId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = "";
        public int CantidadDevuelta { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubtotalDevuelto { get; set; }
    }

    public class DevolucionCreateDto
    {
        [Required(ErrorMessage = "El ID de la venta es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la venta seleccionada no es válido.")]
        public int VentaId { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del usuario no es válido.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El motivo de la devolución es obligatorio.")]
        [StringLength(250, ErrorMessage = "El motivo de la devolución no puede superar los 250 caracteres.")]
        public string Motivo { get; set; } = "";

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public List<DetalleDevolucionCreateDto> Detalles { get; set; } = new();
    }

    public class DetalleDevolucionCreateDto
    {
        [Required(ErrorMessage = "El ID de detalle de venta es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El detalle de venta no es válido.")]
        public int DetalleVentaId { get; set; }

        [Required(ErrorMessage = "El ID de producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El producto no es válido.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad a devolver es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad a devolver debe ser mayor o igual a 1.")]
        public int CantidadDevuelta { get; set; }
    }

    public class DevolucionGestionarDto
    {
        [Required(ErrorMessage = "El nuevo estado es obligatorio.")]
        [Range(2, 3, ErrorMessage = "El nuevo estado debe ser Aprobada (2) o Rechazada (3).")]
        public int NuevoEstado { get; set; }      // 2 = Aprobada | 3 = Rechazada

        [Required(ErrorMessage = "El ID del usuario gestor es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario gestor no es válido.")]
        public int UsuarioGestionId { get; set; }
    }

    public class EstadoDevolucionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public bool Activo { get; set; }
    }
}
