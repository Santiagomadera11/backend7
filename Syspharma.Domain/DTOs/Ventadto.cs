using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class VentaDto
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = null!;
        public int TurnoId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public string? ClienteNombre { get; set; }
        public string? ClienteDocumento { get; set; }
        public string? ClienteTelefono { get; set; }
        public int MetodoPagoId { get; set; }
        public string MetodoPagoNombre { get; set; } = "";
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = "";
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal PorcentajeIva { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
        public DateTime? FechaVenta { get; set; }
        public string Origen { get; set; } = "CAJA";
        public int? PedidoId { get; set; }
        public string? ReferenciasPago { get; set; }
        public List<VentaDetalleDto> Detalles { get; set; } = new();
        public List<VentaDetalleServicioDto> Servicios { get; set; } = new();
    }

    public class VentaDetalleDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal Subtotal { get; set; }
        public int? LoteId { get; set; }
    }

    public class VentaDetalleServicioDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string? ServicioNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal Subtotal { get; set; }
        public int? CitaId { get; set; }
    }

    public class VentaCreateDto
    {
        [Required(ErrorMessage = "El ID del turno es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del turno seleccionado no es válido.")]
        public int TurnoId { get; set; }

        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario seleccionado no es válido.")]
        public int UsuarioId { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del cliente no puede superar los 100 caracteres.")]
        public string? ClienteNombre { get; set; }

        [StringLength(20, ErrorMessage = "El documento del cliente no puede superar los 20 caracteres.")]
        public string? ClienteDocumento { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono del cliente no puede superar los 20 caracteres.")]
        public string? ClienteTelefono { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El método de pago seleccionado no es válido.")]
        public int MetodoPagoId { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0% y 100%.")]
        public decimal PorcentajeIva { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El subtotal no puede ser negativo.")]
        public decimal Subtotal { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El IVA no puede ser negativo.")]
        public decimal Iva { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El total no puede ser negativo.")]
        public decimal Total { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }

        [StringLength(50, ErrorMessage = "El origen no puede superar los 50 caracteres.")]
        public string Origen { get; set; } = "CAJA";

        public int? PedidoId { get; set; }

        [StringLength(100, ErrorMessage = "Las referencias de pago no pueden superar los 100 caracteres.")]
        public string? ReferenciasPago { get; set; }

        public List<VentaDetalleCreateDto> Detalles { get; set; } = new();
        public List<VentaDetalleServicioCreateDto> Servicios { get; set; } = new();
    }

    public class VentaDetalleCreateDto
    {
        [Required(ErrorMessage = "El ID de producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El producto seleccionado no es válido.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo.")]
        public decimal Descuento { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El subtotal no puede ser negativo.")]
        public decimal Subtotal { get; set; }

        public int? LoteId { get; set; }
    }

    public class VentaDetalleServicioCreateDto
    {
        [Required(ErrorMessage = "El ID de servicio es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El servicio seleccionado no es válido.")]
        public int ServicioId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo.")]
        public decimal Descuento { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El subtotal no puede ser negativo.")]
        public decimal Subtotal { get; set; }

        public int? CitaId { get; set; }
    }

    public class VentaUpdateDto
    {
        [Required(ErrorMessage = "El ID de la venta es obligatorio.")]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del cliente no puede superar los 100 caracteres.")]
        public string? ClienteNombre { get; set; }

        [StringLength(20, ErrorMessage = "El documento del cliente no puede superar los 20 caracteres.")]
        public string? ClienteDocumento { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono del cliente no puede superar los 20 caracteres.")]
        public string? ClienteTelefono { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El método de pago seleccionado no es válido.")]
        public int MetodoPagoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El estado seleccionado no es válido.")]
        public int EstadoId { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? Notas { get; set; }
    }

    public class EstadoVentaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}