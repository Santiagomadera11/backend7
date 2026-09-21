using AutoMapper;
using System.Linq;
using Syspharma.Data.Entities;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ==========================================
            // MAPEO DE VENTAS
            // ==========================================
            CreateMap<Venta, VentaDto>()
                // Mapeamos los nombres de las relaciones
                .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src => src.Estado.Nombre))
                .ForMember(dest => dest.MetodoPagoNombre, opt => opt.MapFrom(src => src.MetodoPago.Nombre))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
                // Mapeamos las listas de detalles (Productos y Servicios)
                .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.VentaDetalles))
                .ForMember(dest => dest.Servicios, opt => opt.MapFrom(src => src.VentaDetallesServicios));

            // ==========================================
            // MAPEO DE DETALLES DE PRODUCTOS
            // ==========================================
            CreateMap<VentaDetalle, VentaDetalleDto>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.CostoUnitario, opt => opt.MapFrom(src => CalcularCostoUnitario(src)));

            // ==========================================
            // MAPEO DE DETALLES DE SERVICIOS (NUEVO)
            // ==========================================
            CreateMap<VentaDetalleServicio, VentaDetalleServicioDto>()
                .ForMember(dest => dest.ServicioNombre, opt => opt.MapFrom(src => src.Servicio.Nombre));

            // ==========================================
            // MAPEO PARA ESTADOS DE VENTA
            // ==========================================
            CreateMap<EstadosVentum, EstadoVentaDto>();

            // ==========================================
            // MAPEO PARA CREACI�N (DTO -> Entidad)
            // ==========================================
            // Estos suelen ser autom�ticos si los nombres de propiedades coinciden
            CreateMap<VentaCreateDto, Venta>();
            CreateMap<VentaDetalleCreateDto, VentaDetalle>();
            CreateMap<VentaDetalleServicioCreateDto, VentaDetalleServicio>();

            CreateMap<VentaUpdateDto, Venta>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

        // Costo real de un renglón de venta: promedio ponderado de los lotes de los que
        // salió el stock (costo vigente en ESE momento, no el actual). Si quedó asociado a
        // un único LoteId (formato previo) usa el costo de ese lote; si no hay ningún lote
        // vinculado (dato legado o producto sin lotes), cae al PrecioCompra actual del
        // producto como mejor aproximación disponible.
        private static decimal CalcularCostoUnitario(VentaDetalle d)
        {
            if (d.Lotes != null && d.Lotes.Count > 0)
            {
                var unidades = d.Lotes.Sum(vdl => vdl.Cantidad);
                if (unidades > 0)
                {
                    var costoTotal = d.Lotes.Sum(vdl => vdl.Cantidad * vdl.Lote.CostoUnitario);
                    return costoTotal / unidades;
                }
            }

            if (d.Lote != null) return d.Lote.CostoUnitario;

            return d.Producto?.PrecioCompra ?? 0;
        }
    }
}