using Syspharma.Data.Repositories;
using Syspharma.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syspharma.Business.Services
{
    // INTERFAZ INCLUIDA PARA QUE EL PROGRAM.CS LA ENCUENTRE
    public interface IUsuarioService
    {
        Task<List<UsuarioDto>> ObtenerTodos();
        Task<UsuarioDto?> ObtenerPorId(int id);
        Task<UsuarioDto> Crear(UsuarioCreateDto dto);
        Task<UsuarioDto> Actualizar(UsuarioUpdateDto dto);
        Task<UsuarioDto> CambiarEstado(int id, bool estado);
        Task<bool> Eliminar(int id);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }
        public Task<List<UsuarioDto>> ObtenerTodos() => _repository.ObtenerTodos();
        public Task<UsuarioDto?> ObtenerPorId(int id) => _repository.ObtenerPorId(id);
        public Task<UsuarioDto> Crear(UsuarioCreateDto dto) => _repository.Crear(dto);
        public Task<UsuarioDto> Actualizar(UsuarioUpdateDto dto) => _repository.Actualizar(dto);
        public Task<UsuarioDto> CambiarEstado(int id, bool estado) => _repository.CambiarEstado(id, estado);
        public Task<bool> Eliminar(int id) => _repository.Eliminar(id);
    }
}