using System;
using System.ComponentModel.DataAnnotations;

namespace Syspharma.Domain.DTOs
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Marca { get; set; }
        public string? Presentacion { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public int? ProveedorId { get; set; }
        public string? ProveedorNombre { get; set; }
        public decimal Precio { get; set; }
        public decimal? PrecioCompra { get; set; }
        public decimal PorcentajeIva { get; set; }
        public int Stock { get; set; }
        public string? CodigoBarras { get; set; }
        public string? Imagen { get; set; }
        public bool Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
        public DateOnly? FechaVencimientoProxima { get; set; }

        // --- NUEVAS PROPIEDADES DE MEDICAMENTO ---
        public ProductoMedicamentoDto? Medicamento { get; set; }
        public List<LoteDto> Lotes { get; set; } = new();
    }

    public class ProductoCreateDto
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [StringLength(100, ErrorMessage = "La marca no puede superar los 100 caracteres.")]
        public string? Marca { get; set; }

        [StringLength(100, ErrorMessage = "La presentación no puede superar los 100 caracteres.")]
        public string? Presentacion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La categoría seleccionada no es válida.")]
        public int CategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El proveedor seleccionado no es válido.")]
        public int? ProveedorId { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo.")]
        public decimal Precio { get; set; }

        [Range(0.0, 100.0, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 100.")]
        public decimal PorcentajeIva { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo.")]
        public decimal? PrecioCompra { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock inicial no puede ser negativo.")]
        public int? Stock { get; set; }

        [StringLength(50, ErrorMessage = "El código de barras no puede superar los 50 caracteres.")]
        public string? CodigoBarras { get; set; }

        public string? Imagen { get; set; }

        // --- NUEVAS PROPIEDADES DE MEDICAMENTO ---
        public bool EsMedicamento { get; set; } = false;
        public ProductoMedicamentoDto? Medicamento { get; set; }
    }

    public class ProductoUpdateDto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [StringLength(100, ErrorMessage = "La marca no puede superar los 100 caracteres.")]
        public string? Marca { get; set; }

        [StringLength(100, ErrorMessage = "La presentación no puede superar los 100 caracteres.")]
        public string? Presentacion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La categoría seleccionada no es válida.")]
        public int CategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El proveedor seleccionado no es válido.")]
        public int? ProveedorId { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo.")]
        public decimal Precio { get; set; }

        [Range(0.0, 100.0, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 100.")]
        public decimal PorcentajeIva { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo.")]
        public decimal? PrecioCompra { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int? Stock { get; set; }

        [StringLength(50, ErrorMessage = "El código de barras no puede superar los 50 caracteres.")]
        public string? CodigoBarras { get; set; }

        public string? Imagen { get; set; }

        // --- NUEVAS PROPIEDADES DE MEDICAMENTO ---
        public bool EsMedicamento { get; set; } = false;
        public ProductoMedicamentoDto? Medicamento { get; set; }
    }

    public class LoteDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El número de lote es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de lote no puede superar los 50 caracteres.")]
        public string NumeroLote { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad del lote no puede ser negativa.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateOnly FechaVencimiento { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El costo unitario no puede ser negativo.")]
        public decimal CostoUnitario { get; set; }
    }
}