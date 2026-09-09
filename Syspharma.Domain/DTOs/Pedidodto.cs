using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class PedidoDetalleDto
    {
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PorcentajeIva { get; set; }
        public decimal Iva { get; set; }
    }

    public class PedidoDto
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = null!;
        public int? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public string? ClienteDocumento { get; set; }
        public string? ClienteTelefono { get; set; }
        public string? ClienteEmail { get; set; }
        public string? Direccion { get; set; }
        public int? MetodoPagoId { get; set; }
        public string? MetodoPagoNombre { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
        public string? Origen { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public bool IvaConsistente { get; set; } = true;
        public List<PedidoDetalleDto> Detalles { get; set; } = new();
    }

    public class PedidoDetalleCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto seleccionado no es válido.")]
        public int? ProductoId { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre del producto no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }
    }

    public class PedidoCreateDto
    {
        public int? UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede superar los 100 caracteres.")]
        public string ClienteNombre { get; set; } = null!;

        [StringLength(20, ErrorMessage = "El documento del cliente no puede superar los 20 caracteres.")]
        public string? ClienteDocumento { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? ClienteTelefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string? ClienteEmail { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        public string? Direccion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El método de pago seleccionado no es válido.")]
        public int? MetodoPagoId { get; set; } // Opcional para evitar error 400

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0% y 100%.")]
        public decimal PorcentajeIva { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        [StringLength(50, ErrorMessage = "El origen no puede superar los 50 caracteres.")]
        public string? Origen { get; set; } = "web";

        public DateTime? FechaEntrega { get; set; }

        public List<PedidoDetalleCreateDto> Detalles { get; set; } = new();
        public List<int>? CitaIds { get; set; } = new();
    }

    public class PedidoUpdateDto
    {
        [Required(ErrorMessage = "El ID del pedido es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede superar los 100 caracteres.")]
        public string ClienteNombre { get; set; } = null!;

        [StringLength(20, ErrorMessage = "El documento del cliente no puede superar los 20 caracteres.")]
        public string? ClienteDocumento { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(20, ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
        public string? ClienteTelefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres.")]
        public string? ClienteEmail { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El método de pago seleccionado no es válido.")]
        public int? MetodoPagoId { get; set; }

        [Required(ErrorMessage = "El estado del pedido es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El estado seleccionado no es válido.")]
        public int EstadoId { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        public DateTime? FechaEntrega { get; set; }
        public List<PedidoDetalleCreateDto>? Detalles { get; set; }
    }
}