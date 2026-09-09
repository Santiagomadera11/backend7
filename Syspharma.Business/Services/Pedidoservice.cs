using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;

namespace Syspharma.Business.Services
{
    public interface IPedidoService
    {
        Task<List<PedidoDto>> ObtenerTodos();
        Task<PedidoDto?> ObtenerPorId(int id);
        Task<PedidoDto> Crear(PedidoCreateDto dto);
        Task<PedidoDto> Actualizar(PedidoUpdateDto dto);
        Task<bool> CambiarEstado(int id, int estadoId);
        Task<bool> Eliminar(int id);
        Task<List<object>> ObtenerEstados();
    }

    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repo;
        private readonly IVentaService _ventaService;
        private readonly SyspharmaContext _context;

        // ─── ID del estado "Entregado" en la tabla estados_pedido ───────────────
        // Si tu tabla usa otro nombre exacto, cámbialo aquí.
        private const string ESTADO_ENTREGADO_NOMBRE = "Entregado";

        public PedidoService(
            IPedidoRepository repo,
            IVentaService ventaService,
            SyspharmaContext context)
        {
            _repo = repo;
            _ventaService = ventaService;
            _context = context;
        }

        public async Task<List<PedidoDto>> ObtenerTodos()
        {
            try { return await _repo.ObtenerTodos(); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico en PedidoService: {ex.Message}");
                return new List<PedidoDto>();
            }
        }

        public Task<PedidoDto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public Task<PedidoDto> Crear(PedidoCreateDto dto) => _repo.Crear(dto);
        public Task<PedidoDto> Actualizar(PedidoUpdateDto dto) => _repo.Actualizar(dto);
        public Task<bool> Eliminar(int id) => _repo.Eliminar(id);
        public Task<List<object>> ObtenerEstados() => _repo.ObtenerEstados();

        // ────────────────────────────────────────────────────────────────────────
        // CAMBIAR ESTADO — intercepta "Entregado" para disparar la auto-venta
        // ────────────────────────────────────────────────────────────────────────
        public async Task<bool> CambiarEstado(int id, int estadoId)
        {
            // Verificamos si el estado destino es "Entregado"
            var estadoDestino = await _context.EstadosPedidos
                .FirstOrDefaultAsync(e => e.Id == estadoId);

            if (estadoDestino == null)
                throw new Exception($"El estado con ID {estadoId} no existe.");

            bool esEntregado = estadoDestino.Nombre
                .Equals(ESTADO_ENTREGADO_NOMBRE, StringComparison.OrdinalIgnoreCase);

            if (esEntregado)
            {
                // Verificar que no se haya convertido ya (evitar duplicados)
                var yaConvertido = await _context.Ventas
                    .AnyAsync(v => v.PedidoId == id);

                if (yaConvertido)
                    throw new Exception("Este pedido ya fue convertido a venta anteriormente.");

                // Crear la venta automáticamente
                await _ventaService.CrearDesdePedido(id);
            }

            // Actualizar el estado del pedido en el repo (como siempre)
            return await _repo.CambiarEstado(id, estadoId);
        }
    }
}