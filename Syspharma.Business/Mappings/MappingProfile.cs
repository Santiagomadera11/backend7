using AutoMapper;
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
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre));

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
            // MAPEO PARA CREACIÓN (DTO -> Entidad)
            // ==========================================
            // Estos suelen ser automáticos si los nombres de propiedades coinciden
            CreateMap<VentaCreateDto, Venta>();
            CreateMap<VentaDetalleCreateDto, VentaDetalle>();
            CreateMap<VentaDetalleServicioCreateDto, VentaDetalleServicio>();

            CreateMap<VentaUpdateDto, Venta>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}