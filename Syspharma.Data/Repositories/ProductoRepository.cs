using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syspharma.Data.Repositories
{
    public interface IProductoRepository
    {
        Task<List<ProductoDto>> ObtenerTodos();
        Task<ProductoDto?> ObtenerPorId(int id);
        Task<ProductoDto> Crear(ProductoCreateDto dto);
        Task<ProductoDto> Actualizar(ProductoUpdateDto dto);
        Task<bool> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
        Task<List<ProductoDto>> ProximosAVencer(int dias);
        Task<List<ProductoPublicoDto>> ObtenerCatalogoPublico();
    }

    public class ProductoRepository : IProductoRepository
    {
        private readonly SyspharmaContext _context;

        public ProductoRepository(SyspharmaContext context)
        {
            _context = context;
        }

        private static ProductoDto MapToDto(Producto p)
        {
            var activeLotes = p.Lotes?.Where(l => l.FechaVencimiento >= DateOnly.FromDateTime(DateTime.Today) && l.Cantidad > 0).ToList();

            return new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                MarcaId = p.MarcaId,
                MarcaNombre = p.Marca?.Nombre,
                PresentacionId = p.PresentacionId,
                PresentacionNombre = p.Presentacion?.Nombre,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria?.Nombre,
                ProveedorId = p.ProveedorId,
                ProveedorNombre = p.Proveedor?.Nombre,
                Precio = p.Precio,
                PrecioCompra = p.PrecioCompra,
                PorcentajeIva = p.PorcentajeIva,
                Stock = activeLotes != null && activeLotes.Any() ? activeLotes.Sum(l => l.Cantidad) : p.Stock,
                CodigoBarras = p.CodigoBarras,
                Imagen = p.Imagen,
                Estado = p.Estado,
                FechaCreacion = p.FechaCreacion,
                UltimaActualizacion = p.UltimaActualizacion,
                FechaVencimientoProxima = activeLotes != null && activeLotes.Any() ? activeLotes.Min(l => l.FechaVencimiento) : p.FechaVencimientoProxima,
                Medicamento = p.ProductoMedicamento != null ? new ProductoMedicamentoDto
                {
                    Id = p.ProductoMedicamento.Id,
                    ProductoId = p.ProductoMedicamento.ProductoId,
                    Composicion = p.ProductoMedicamento.Composicion,
                    Concentracion = p.ProductoMedicamento.Concentracion,
                    ViaAdministracion = p.ProductoMedicamento.ViaAdministracion,
                    RegistroSanitario = p.ProductoMedicamento.RegistroSanitario,
                    RequiereFormula = p.ProductoMedicamento.RequiereFormula,
                    Indicaciones = p.ProductoMedicamento.Indicaciones,
                    Posologia = p.ProductoMedicamento.Posologia,
                    UnidadesPorEnvase = p.ProductoMedicamento.UnidadesPorEnvase,
                    RequiereRefrigeracion = p.ProductoMedicamento.RequiereRefrigeracion,
                    AfectaConduccion = p.ProductoMedicamento.AfectaConduccion,
                    Fotosensible = p.ProductoMedicamento.Fotosensible
                } : null,
                Lotes = activeLotes?.Select(l => new LoteDto
                {
                    Id = l.Id,
                    ProductoId = l.ProductoId,
                    NumeroLote = l.NumeroLote,
                    Cantidad = l.Cantidad,
                    FechaVencimiento = l.FechaVencimiento,
                    CostoUnitario = l.CostoUnitario
                }).ToList() ?? new()
            };
        }

        public async Task<List<ProductoDto>> ObtenerTodos()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Include(p => p.Marca)
                .Include(p => p.Presentacion)
                .Include(p => p.ProductoMedicamento)
                .Include(p => p.Lotes)
                .ToListAsync();

            return productos.Select(MapToDto).ToList();
        }

        public async Task<ProductoDto?> ObtenerPorId(int id)
        {
            var p = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Include(p => p.Marca)
                .Include(p => p.Presentacion)
                .Include(p => p.ProductoMedicamento)
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.Id == id);

            return p == null ? null : MapToDto(p);
        }

        public async Task<List<ProductoPublicoDto>> ObtenerCatalogoPublico()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Presentacion)
                .Include(p => p.ProductoMedicamento)
                .Where(p => p.Estado == true)
                .ToListAsync();

            return productos.Select(p => new ProductoPublicoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Stock = p.Stock,
                Imagen = p.Imagen,
                Categoria = p.Categoria?.Nombre ?? "Sin categoría",
                Marca = p.Marca?.Nombre ?? "",
                Presentacion = p.Presentacion?.Nombre ?? "",
                TipoProducto = p.ProductoMedicamento != null ? "Medicamento" : "Producto General",
                Composicion = p.ProductoMedicamento?.Composicion ?? "",
                Concentracion = p.ProductoMedicamento?.Concentracion ?? "",
                ViaAdministracion = p.ProductoMedicamento?.ViaAdministracion ?? "",
                RegistroSanitario = p.ProductoMedicamento?.RegistroSanitario ?? "",
                RequiereFormula = p.ProductoMedicamento?.RequiereFormula ?? false,
                Estado = p.Estado,
            }).ToList();
        }

        public async Task<List<ProductoDto>> ProximosAVencer(int dias)
        {
            var limite = DateOnly.FromDateTime(DateTime.Today.AddDays(dias));
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Include(p => p.Marca)
                .Include(p => p.Presentacion)
                .Include(p => p.ProductoMedicamento)
                .Include(p => p.Lotes)
                .Where(p => p.Estado == true)
                .ToListAsync();

            return productos
                .Select(MapToDto)
                .Where(p => p.FechaVencimientoProxima != null && p.FechaVencimientoProxima <= limite)
                .OrderBy(p => p.FechaVencimientoProxima)
                .ToList();
        }

        public async Task<ProductoDto> Crear(ProductoCreateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.CodigoBarras))
            {
                var yaExiste = await _context.Productos.AnyAsync(p => p.CodigoBarras == dto.CodigoBarras);
                if (yaExiste) throw new Exception("Ya existe un producto con ese código de barras.");
            }

            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                MarcaId = dto.MarcaId,
                PresentacionId = dto.PresentacionId,
                CategoriaId = dto.CategoriaId,
                ProveedorId = dto.ProveedorId,
                Precio = dto.Precio,
                PrecioCompra = dto.PrecioCompra,
                PorcentajeIva = dto.PorcentajeIva,
                Stock = dto.Stock ?? 0,
                CodigoBarras = dto.CodigoBarras,
                Imagen = dto.Imagen,
                Estado = true,
                FechaCreacion = DateTime.Now,
                UltimaActualizacion = DateTime.Now
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // --- NUEVO: Guardado de los detalles de medicamento ---
            if (dto.EsMedicamento && dto.Medicamento != null)
            {
                var medicamento = new ProductoMedicamento
                {
                    ProductoId = producto.Id,
                    Composicion = dto.Medicamento.Composicion,
                    Concentracion = dto.Medicamento.Concentracion,
                    ViaAdministracion = dto.Medicamento.ViaAdministracion,
                    RegistroSanitario = dto.Medicamento.RegistroSanitario,
                    RequiereFormula = dto.Medicamento.RequiereFormula,
                    Indicaciones = dto.Medicamento.Indicaciones,
                    Posologia = dto.Medicamento.Posologia,
                    UnidadesPorEnvase = dto.Medicamento.UnidadesPorEnvase,
                    RequiereRefrigeracion = dto.Medicamento.RequiereRefrigeracion,
                    AfectaConduccion = dto.Medicamento.AfectaConduccion,
                    Fotosensible = dto.Medicamento.Fotosensible
                };

                _context.ProductoMedicamentos.Add(medicamento);
                await _context.SaveChangesAsync();
            }

            return await ObtenerPorId(producto.Id) ?? MapToDto(producto);
        }

        public async Task<ProductoDto> Actualizar(ProductoUpdateDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.Id);

            if (producto == null)
                throw new Exception("Producto no encontrado");

            if (!string.IsNullOrWhiteSpace(dto.CodigoBarras))
            {
                var yaExiste = await _context.Productos.AnyAsync(p => p.CodigoBarras == dto.CodigoBarras && p.Id != dto.Id);
                if (yaExiste) throw new Exception("Ya existe un producto con ese código de barras.");
            }

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.MarcaId = dto.MarcaId;
            producto.PresentacionId = dto.PresentacionId;
            producto.CategoriaId = dto.CategoriaId;
            producto.ProveedorId = dto.ProveedorId;
            producto.Precio = dto.Precio;
            producto.PrecioCompra = dto.PrecioCompra;
            producto.PorcentajeIva = dto.PorcentajeIva;
            producto.Stock = dto.Stock ?? producto.Stock;
            producto.CodigoBarras = dto.CodigoBarras;
            producto.Imagen = dto.Imagen;
            producto.UltimaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            // --- NUEVO: Gestión de actualización del detalle de medicamento ---
            var medicamentoExistente = await _context.ProductoMedicamentos
                .FirstOrDefaultAsync(pm => pm.ProductoId == producto.Id);

            if (dto.EsMedicamento && dto.Medicamento != null)
            {
                if (medicamentoExistente == null)
                {
                    // Si antes era producto normal y ahora es medicamento, creamos el detalle
                    var nuevoMedicamento = new ProductoMedicamento
                    {
                        ProductoId = producto.Id,
                        Composicion = dto.Medicamento.Composicion,
                        Concentracion = dto.Medicamento.Concentracion,
                        ViaAdministracion = dto.Medicamento.ViaAdministracion,
                        RegistroSanitario = dto.Medicamento.RegistroSanitario,
                        RequiereFormula = dto.Medicamento.RequiereFormula,
                        Indicaciones = dto.Medicamento.Indicaciones,
                        Posologia = dto.Medicamento.Posologia,
                        UnidadesPorEnvase = dto.Medicamento.UnidadesPorEnvase,
                        RequiereRefrigeracion = dto.Medicamento.RequiereRefrigeracion,
                        AfectaConduccion = dto.Medicamento.AfectaConduccion,
                        Fotosensible = dto.Medicamento.Fotosensible
                    };
                    _context.ProductoMedicamentos.Add(nuevoMedicamento);
                }
                else
                {
                    // Si ya existía, actualizamos sus campos adicionales
                    medicamentoExistente.Composicion = dto.Medicamento.Composicion;
                    medicamentoExistente.Concentracion = dto.Medicamento.Concentracion;
                    medicamentoExistente.ViaAdministracion = dto.Medicamento.ViaAdministracion;
                    medicamentoExistente.RegistroSanitario = dto.Medicamento.RegistroSanitario;
                    medicamentoExistente.RequiereFormula = dto.Medicamento.RequiereFormula;
                    medicamentoExistente.Indicaciones = dto.Medicamento.Indicaciones;
                    medicamentoExistente.Posologia = dto.Medicamento.Posologia;
                    medicamentoExistente.UnidadesPorEnvase = dto.Medicamento.UnidadesPorEnvase;
                    medicamentoExistente.RequiereRefrigeracion = dto.Medicamento.RequiereRefrigeracion;
                    medicamentoExistente.AfectaConduccion = dto.Medicamento.AfectaConduccion;
                    medicamentoExistente.Fotosensible = dto.Medicamento.Fotosensible;
                }
                await _context.SaveChangesAsync();
            }
            else if (!dto.EsMedicamento && medicamentoExistente != null)
            {
                // Si ya no es medicamento, removemos el detalle de la base de datos
                _context.ProductoMedicamentos.Remove(medicamentoExistente);
                await _context.SaveChangesAsync();
            }

            return await ObtenerPorId(producto.Id) ?? MapToDto(producto);
        }

        public async Task<bool> CambiarEstado(int id, bool estado)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            producto.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            var enVentas = await _context.VentaDetalles.AnyAsync(d => d.ProductoId == id);
            var enPedidos = await _context.PedidoDetalles.AnyAsync(d => d.ProductoId == id);
            var enCompras = await _context.CompraDetalles.AnyAsync(d => d.ProductoId == id);
            var enDevoluciones = await _context.DetallesDevoluciones.AnyAsync(d => d.ProductoId == id);

            if (enVentas || enPedidos || enCompras || enDevoluciones)
            {
                throw new Exception("No se puede eliminar el producto porque está registrado en transacciones de venta, compra, devolución o pedidos. Desactívelo en su lugar.");
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
