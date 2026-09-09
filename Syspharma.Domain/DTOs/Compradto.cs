using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class CompraDetalleDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? Lote { get; set; }
        public DateOnly? FechaVencimiento { get; set; }
    }

    public class CompraDto
    {
        public int Id { get; set; }
        public string NumeroCompra { get; set; } = null!;
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = null!;
        public string? ProveedorDocumento { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? FechaCompra { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public List<CompraDetalleDto> Detalles { get; set; } = new();
    }

    public class CompraDetalleCreateDto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El producto seleccionado no es válido.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario (costo) es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        [StringLength(50, ErrorMessage = "El lote no puede superar los 50 caracteres.")]
        public string? Lote { get; set; }

        public DateOnly? FechaVencimiento { get; set; }
    }

    public class CompraCreateDto
    {
        [Required(ErrorMessage = "El ID de proveedor es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El proveedor seleccionado no es válido.")]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El usuario seleccionado no es válido.")]
        public int UsuarioId { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0% y 100%.")]
        public decimal? PorcentajeIva { get; set; } = 19;

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public List<CompraDetalleCreateDto> Detalles { get; set; } = new();
    }

    public class CompraUpdateDto
    {
        [Required(ErrorMessage = "El ID de la compra es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID de proveedor es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El proveedor seleccionado no es válido.")]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El estado de la compra es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El estado seleccionado no es válido.")]
        public int EstadoId { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public DateTime? FechaEntrega { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0% y 100%.")]
        public decimal PorcentajeIva { get; set; } = 19;

        public List<CompraDetalleCreateDto> Detalles { get; set; } = new();
    }
}
