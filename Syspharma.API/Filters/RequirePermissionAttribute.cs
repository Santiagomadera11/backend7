using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Syspharma.Data.Context;

namespace Syspharma.API.Filters
{
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string permission) : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class RequirePermissionFilter : IAsyncActionFilter
    {
        private readonly SyspharmaContext _context;
        private readonly string _permission;

        public RequirePermissionFilter(SyspharmaContext context, string permission)
        {
            _context = context;
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolesPermisos)
                        .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var esAdmin = usuario.Role?.Nombre?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ?? false;
            var tienePermiso = esAdmin || (usuario.Role?.RolesPermisos?.Any(rp => 
                rp.Permiso.Codigo.Equals(_permission, StringComparison.OrdinalIgnoreCase)) ?? false);

            if (!tienePermiso)
            {
                context.Result = new ObjectResult(new { message = $"No tienes el permiso requerido: {_permission}" }) 
                { 
                    StatusCode = StatusCodes.Status403Forbidden 
                };
                return;
            }

            await next();
        }
    }
}
