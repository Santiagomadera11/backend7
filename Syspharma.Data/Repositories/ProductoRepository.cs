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
                }).ToList() ?? new(),
                FormasVenta = p.FormasVenta?.Where(f => f.Activo).Select(f => new ProductoFormaVentaDto
                {
                    Id = f.Id,
                    Tipo = f.Tipo,
                    Precio = f.Precio,
                    FactorUnidades = f.FactorUnidades,
                    Activo = f.Activo
                }).ToList() ?? new()
            };
        }

        // Garantiza que exista SIEMPRE una forma "Unidad" con Precio = producto.Precio y
        // FactorUnidades = 1 (el precio del producto es la fuente de verdad para Unidad,
        // nunca un valor independiente enviado por el frontend). Valida Blister/Caja.
        private static List<ProductoFormaVentaDto> NormalizarFormasVenta(List<ProductoFormaVentaDto>? formas, decimal precioProducto)
        {
            var normalizadas = new List<ProductoFormaVentaDto>();
            var tiposVistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (formas != null)
            {
                foreach (var f in formas)
                {
                    if (string.IsNullOrWhiteSpace(f.Tipo)) continue;
                    var tipo = f.Tipo.Trim();

                    if (!tiposVistos.Add(tipo))
                        throw new Exception($"La forma de venta '{tipo}' está duplicada.");

                    if (tipo.Equals("Unidad", StringComparison.OrdinalIgnoreCase))
                    {
                        normalizadas.Add(new ProductoFormaVentaDto
                        {
                            Id = f.Id,
                            Tipo = "Unidad",
                            Precio = precioProducto,
                            FactorUnidades = 1,
                            Activo = true
                        });
                    }
                    else
                    {
                        if (f.FactorUnidades < 1)
                            throw new Exception($"El factor de unidades para '{tipo}' debe ser mayor o igual a 1.");
                        if (f.Precio <= 0)
                            throw new Exception($"El precio para '{tipo}' debe ser mayor a 0.");

                        normalizadas.Add(new ProductoFormaVentaDto
                        {
                            Id = f.Id,
                            Tipo = tipo,
                            Precio = f.Precio,
                            FactorUnidades = f.FactorUnidades,
                            Activo = true
                        });
                    }
                }
            }

            if (!tiposVistos.Contains("Unidad"))
            {
                normalizadas.Add(new ProductoFormaVentaDto
                {
                    Tipo = "Unidad",
                    Precio = precioProducto,
                    FactorUnidades = 1,
                    Activo = true
                });
            }

            return normalizadas;
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
                .Include(p => p.FormasVenta)
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
                .Include(p => p.FormasVenta)
                .FirstOrDefaultAsync(p => p.Id == id);

            return p == null ? null : MapToDto(p);
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
                .Include(p => p.FormasVenta)
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

            // --- FORMAS DE VENTA (Unidad/Blister/Caja) ---
            var formasNormalizadas = NormalizarFormasVenta(dto.FormasVenta, producto.Precio);
            foreach (var f in formasNormalizadas)
            {
                _context.ProductoFormasVenta.Add(new ProductoFormaVenta
                {
                    ProductoId = producto.Id,
                    Tipo = f.Tipo,
                    Precio = f.Precio,
                    FactorUnidades = f.FactorUnidades,
                    Activo = true
                });
            }
            await _context.SaveChangesAsync();

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

            // --- FORMAS DE VENTA (Unidad/Blister/Caja) ---
            // Actualiza in-place las que coinciden por Tipo (nunca borra el Id, referenciado
            // por ventas/pedidos históricos). Las que ya no vienen en el DTO se desactivan
            // (soft-delete): nunca se hace Remove() de una fila producto_forma_venta.
            var formasNormalizadas = NormalizarFormasVenta(dto.FormasVenta, producto.Precio);
            var formasExistentes = await _context.ProductoFormasVenta
                .Where(f => f.ProductoId == producto.Id)
                .ToListAsync();

            var tiposEnviados = new HashSet<string>(formasNormalizadas.Select(f => f.Tipo), StringComparer.OrdinalIgnoreCase);

            foreach (var f in formasNormalizadas)
            {
                var existente = formasExistentes.FirstOrDefault(e => e.Tipo.Equals(f.Tipo, StringComparison.OrdinalIgnoreCase));
                if (existente != null)
                {
                    existente.Precio = f.Precio;
                    existente.FactorUnidades = f.FactorUnidades;
                    existente.Activo = true;
                }
                else
                {
                    _context.ProductoFormasVenta.Add(new ProductoFormaVenta
                    {
                        ProductoId = producto.Id,
                        Tipo = f.Tipo,
                        Precio = f.Precio,
                        FactorUnidades = f.FactorUnidades,
                        Activo = true
                    });
                }
            }

            foreach (var existente in formasExistentes)
            {
                if (!tiposEnviados.Contains(existente.Tipo))
                {
                    existente.Activo = false; // soft-delete: nunca Remove()
                }
            }

            await _context.SaveChangesAsync();

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
            var enCompras = await _context.CompraDetalles.AnyAsync(d => d.ProductoId == id);
            var enDevoluciones = await _context.DetallesDevoluciones.AnyAsync(d => d.ProductoId == id);

            if (enVentas || enCompras || enDevoluciones)
            {
                throw new Exception("No se puede eliminar el producto porque está registrado en transacciones de venta, compra o devolución. Desactívelo en su lugar.");
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
