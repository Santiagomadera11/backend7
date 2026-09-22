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
            CreateMap<Venta, VentaDto>()
                .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src => src.Estado.Nombre))
                .ForMember(dest => dest.MetodoPagoNombre, opt => opt.MapFrom(src => src.MetodoPago.Nombre))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario.Nombre))
                .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.VentaDetalles))
                .ForMember(dest => dest.Servicios, opt => opt.MapFrom(src => src.VentaDetallesServicios));

            CreateMap<VentaDetalle, VentaDetalleDto>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.CostoUnitario, opt => opt.MapFrom(src => CalcularCostoUnitario(src)));

            CreateMap<VentaDetalleServicio, VentaDetalleServicioDto>()
                .ForMember(dest => dest.ServicioNombre, opt => opt.MapFrom(src => src.Servicio.Nombre));

            CreateMap<EstadosVentum, EstadoVentaDto>();

            CreateMap<VentaCreateDto, Venta>();
            CreateMap<VentaDetalleCreateDto, VentaDetalle>();
            CreateMap<VentaDetalleServicioCreateDto, VentaDetalleServicio>();

            CreateMap<VentaUpdateDto, Venta>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

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