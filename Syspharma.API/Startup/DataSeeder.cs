using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syspharma.Data.Context;
using Syspharma.Data.Entities;

namespace Syspharma.API.Startup
{
    /// <summary>
    /// Datos mínimos para que la aplicación sea usable en una base de datos nueva:
    /// roles, catálogo de permisos y un usuario Administrador. Idempotente: cada
    /// bloque solo inserta si la tabla correspondiente está vacía.
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SyspharmaContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

            var roleAdmin = await SeedRolesAsync(context);
            await SeedPermisosAsync(context, roleAdmin);
            await SeedCatalogosAsync(context);
            await SeedMarcasAsync(context);
            await SeedPresentacionesAsync(context);
            await SeedCategoriasMedicamentosAsync(context);
            await SeedAdminUserAsync(userManager, context, roleAdmin);
        }

        // Marcas de arranque de ejemplo — el admin puede agregar/editar/eliminar desde
        // Inventario > Marcas. No representan un catálogo oficial ni datos recuperados.
        private static async Task SeedMarcasAsync(SyspharmaContext context)
        {
            if (await context.Marcas.AnyAsync()) return;

            var nombres = new[] { "Genfar", "MK", "La Santé", "Tecnoquímicas", "Bayer", "Pfizer", "Procaps", "Baxter" };
            context.Marcas.AddRange(nombres.Select(n => new Marca { Nombre = n, Estado = true, FechaCreacion = DateTime.Now }));
            await context.SaveChangesAsync();
        }

        // Presentaciones de arranque de ejemplo — el admin puede agregar/editar/eliminar
        // desde Inventario > Presentaciones. No representan un catálogo oficial.
        private static async Task SeedPresentacionesAsync(SyspharmaContext context)
        {
            if (await context.Presentaciones.AnyAsync()) return;

            var nombres = new[] {
                "Tableta", "Cápsula", "Jarabe", "Suspensión", "Solución Inyectable",
                "Crema", "Gel", "Ungüento", "Gotas", "Óvulos", "Supositorio", "Inhalador"
            };
            context.Presentaciones.AddRange(nombres.Select(n => new Presentacion { Nombre = n, Estado = true, FechaCreacion = DateTime.Now }));
            await context.SaveChangesAsync();
        }

        // Categorías farmacológicas solicitadas por el administrador, con su
        // descripción para uso en el catálogo. Idempotente por nombre: solo
        // inserta las que todavía no existan, así se puede correr en cualquier
        // despliegue sin duplicar ni pisar categorías ya creadas manualmente.
        private static readonly (string Nombre, string Descripcion)[] CategoriasMedicamentos = new[]
        {
            ("Antipiréticos", "Su única función principal es bajar la fiebre. Ayudan a que el termostato de tu cuerpo regrese a su temperatura normal cuando estás enfermo."),
            ("Antibióticos", "Son armas exclusivas para combatir y destruir a las bacterias. No sirven para los virus, por lo que solo se usan en infecciones bacterianas como la amigdalitis o infecciones urinarias."),
            ("Antivirales", "Estos medicamentos frenan o detienen la reproducción de los virus dentro de tu cuerpo. Se usan para enfermedades como la gripe común, la varicela o el herpes."),
            ("Antifúngicos (o Antimicóticos)", "Se encargan de eliminar los hongos. Son los que se usan para curar problemas comunes como el pie de atleta o los hongos en las uñas."),
            ("Antihistamínicos", "Bloquean una sustancia llamada histamina que el cuerpo libera durante las alergias. Sirven para frenar los estornudos, la picazón en los ojos y los mocos transparentes."),
            ("Antitusivos", "Son medicamentos que calman el reflejo de la tos seca. Le avisan al cerebro que detenga el estímulo de toser cuando no hay flemas que expulsar."),
            ("Mucolíticos", "Hacen que los mocos y flemas atrapados en el pecho o la nariz se vuelvan más delgados y líquidos. Esto ayuda a que los puedas expulsar mucho más fácil al toser."),
            ("Antiácidos", "Funcionan como un escudo que neutraliza los ácidos del estómago. Alivian rápidamente esa sensación de quemazón o agrieras que sube por el pecho después de comer algo pesado."),
            ("Antidiarréicos", "Ayudan a reducir los movimientos rápidos del intestino para frenar la diarrea. Hacen que el cuerpo absorba mejor los líquidos para evitar que te deshidrates."),
            ("Laxantes", "Hacen el trabajo contrario a los anteriores; ablandan las heces o estimulan el intestino para ayudar a evacuar cuando sufres de estreñimiento o dificultad para ir al baño."),
            ("Antidepresivos", "Son medicamentos que ayudan a balancear los químicos del cerebro. Se usan bajo estricta vigilancia médica para mejorar el estado de ánimo en personas con depresión."),
            ("Ansiolíticos", "Ayudan a calmar el sistema nervioso para disminuir los ataques de ansiedad o pánico. Suelen relajar el cuerpo y, en muchos casos, ayudan a conciliar el sueño."),
            ("Antihipertensivos", "Son medicinas que relajan los vasos sanguíneos para que la sangre fluya con menos esfuerzo. Su meta es mantener la presión arterial en niveles seguros y proteger el corazón."),
        };

        private static async Task SeedCategoriasMedicamentosAsync(SyspharmaContext context)
        {
            var existentes = await context.Categorias
                .Select(c => c.Nombre)
                .ToListAsync();
            var existentesSet = new HashSet<string>(existentes, StringComparer.OrdinalIgnoreCase);

            var faltantes = CategoriasMedicamentos
                .Where(c => !existentesSet.Contains(c.Nombre))
                .Select(c => new Categoria
                {
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                })
                .ToList();

            if (faltantes.Count > 0)
            {
                context.Categorias.AddRange(faltantes);
                await context.SaveChangesAsync();
            }
        }

        // Nombres y orden de Id tomados de comparaciones/valores hardcodeados encontrados
        // en el código (backend y frontend) — ver detalle en el mensaje de la conversación.
        // Donde la evidencia era contradictoria (EstadosCita) se sembró un superset seguro:
        // el código no falla por tener catálogos de más, sí por no encontrar el nombre que busca.
        private static async Task SeedCatalogosAsync(SyspharmaContext context)
        {
            if (!await context.EstadosVenta.AnyAsync())
            {
                context.EstadosVenta.AddRange(
                    new EstadosVentum { Nombre = "Completada" },   // id 1: usado como estado inicial de toda venta
                    new EstadosVentum { Nombre = "Devolución" },   // id 2
                    new EstadosVentum { Nombre = "Anulada" }       // id 3
                );
            }

            if (!await context.EstadosDevoluciones.AnyAsync())
            {
                context.EstadosDevoluciones.AddRange(
                    new EstadoDevolucion { Nombre = "Pendiente", Activo = true },  // id 1
                    new EstadoDevolucion { Nombre = "Aprobada", Activo = true },   // id 2
                    new EstadoDevolucion { Nombre = "Rechazada", Activo = true }   // id 3
                );
            }

            if (!await context.EstadosCompras.AnyAsync())
            {
                context.EstadosCompras.AddRange(
                    new EstadosCompra { Nombre = "Pendiente" },
                    new EstadosCompra { Nombre = "En Camino" },
                    new EstadosCompra { Nombre = "Recibida" },
                    new EstadosCompra { Nombre = "Cancelada" }
                );
            }

            if (!await context.EstadosProveedors.AnyAsync())
            {
                context.EstadosProveedors.AddRange(
                    new EstadosProveedor { Nombre = "Activo" },
                    new EstadosProveedor { Nombre = "Inactivo" }
                );
            }

            if (!await context.EstadosCita.AnyAsync())
            {
                // Evidencia contradictoria entre EmployeeCitas.jsx y ClientMisCitas.jsx:
                // se incluye la unión de ambos sets. Revisar y depurar manualmente
                // una vez confirmado cuál es el flujo de estados real de citas.
                context.EstadosCita.AddRange(
                    new EstadosCitum { Nombre = "Confirmar Asistencia" }, // id 1: asignado por defecto al crear cita
                    new EstadosCitum { Nombre = "Confirmada" },
                    new EstadosCitum { Nombre = "En Consulta" },
                    new EstadosCitum { Nombre = "Completada" },
                    new EstadosCitum { Nombre = "Cancelada" },
                    new EstadosCitum { Nombre = "No Asistió" },
                    new EstadosCitum { Nombre = "Pagada" }
                );
            }

            if (!await context.MetodosPagos.AnyAsync())
            {
                context.MetodosPagos.AddRange(
                    new MetodosPago { Nombre = "Efectivo", Estado = true, FechaCreacion = DateTime.Now },
                    new MetodosPago { Nombre = "Tarjeta Débito/Crédito", Estado = true, FechaCreacion = DateTime.Now },
                    new MetodosPago { Nombre = "Transferencia Bancaria", Estado = true, FechaCreacion = DateTime.Now },
                    new MetodosPago { Nombre = "Nequi", Estado = true, FechaCreacion = DateTime.Now },
                    new MetodosPago { Nombre = "Daviplata", Estado = true, FechaCreacion = DateTime.Now }
                );
            }

            if (!await context.TiposDocumentos.AnyAsync())
            {
                context.TiposDocumentos.AddRange(
                    new TiposDocumento { Nombre = "Cédula de Ciudadanía", Estado = true, FechaCreacion = DateTime.Now },
                    new TiposDocumento { Nombre = "Cédula de Extranjería", Estado = true, FechaCreacion = DateTime.Now },
                    new TiposDocumento { Nombre = "NIT", Estado = true, FechaCreacion = DateTime.Now },
                    new TiposDocumento { Nombre = "Pasaporte", Estado = true, FechaCreacion = DateTime.Now },
                    new TiposDocumento { Nombre = "RUT", Estado = true, FechaCreacion = DateTime.Now },
                    new TiposDocumento { Nombre = "Tarjeta de Identidad", Estado = true, FechaCreacion = DateTime.Now }
                );
            }

            if (!await context.CategoriaServicios.AnyAsync())
            {
                context.CategoriaServicios.AddRange(
                    new CategoriaServicio { Nombre = "Consulta Médica", Estado = true, FechaCreacion = DateTime.Now },
                    new CategoriaServicio { Nombre = "Procedimiento", Estado = true, FechaCreacion = DateTime.Now },
                    new CategoriaServicio { Nombre = "Examen de Laboratorio", Estado = true, FechaCreacion = DateTime.Now },
                    new CategoriaServicio { Nombre = "Vacunación", Estado = true, FechaCreacion = DateTime.Now },
                    new CategoriaServicio { Nombre = "Otro", Estado = true, FechaCreacion = DateTime.Now }
                );
            }

            await context.SaveChangesAsync();
        }

        private static async Task<Role> SeedRolesAsync(SyspharmaContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { Nombre = "Administrador", Descripcion = "Acceso total al sistema", Estado = true, FechaCreacion = DateTime.Now },
                    new Role { Nombre = "Empleado", Descripcion = "Personal operativo", Estado = true, FechaCreacion = DateTime.Now }
                );
                await context.SaveChangesAsync();
            }

            return await context.Roles.FirstAsync(r => r.Nombre == "Administrador");
        }

        // Códigos tomados de Syspharma2/src/features/settings/rolesConfig.js (PERMISSIONS_CONFIG),
        // que es la fuente de verdad que usa el frontend para administrar permisos por rol.
        private static readonly (string Codigo, string Nombre, string Categoria)[] PermisosCatalogo = new[]
        {
            ("dashboard.view", "Acceso al Dashboard", "Inicio"),

            ("users.view", "Ver usuarios", "Usuarios"),
            ("users.create", "Agregar usuarios", "Usuarios"),
            ("users.edit", "Editar usuarios", "Usuarios"),
            ("users.delete", "Eliminar usuarios", "Usuarios"),
            ("users.status", "Cambiar estado", "Usuarios"),

            ("purchase.view", "Ver compras", "Compras"),
            ("purchase.create", "Agregar compra", "Compras"),
            ("purchase.edit", "Editar compra", "Compras"),
            ("purchase.delete", "Eliminar compra", "Compras"),
            ("purchase.status", "Cambiar estado", "Compras"),

            ("products.view", "Ver productos", "Productos"),
            ("products.create", "Agregar producto", "Productos"),
            ("products.edit", "Editar producto", "Productos"),
            ("products.delete", "Eliminar producto", "Productos"),
            ("products.status", "Cambiar estado", "Productos"),

            ("categories.view", "Ver categorías", "Categorías"),
            ("categories.create", "Agregar categoría", "Categorías"),
            ("categories.edit", "Editar categoría", "Categorías"),
            ("categories.delete", "Eliminar categoría", "Categorías"),
            ("categories.status", "Cambiar estado", "Categorías"),

            ("brands.view", "Ver marcas", "Marcas"),
            ("brands.create", "Agregar marca", "Marcas"),
            ("brands.edit", "Editar marca", "Marcas"),
            ("brands.delete", "Eliminar marca", "Marcas"),
            ("brands.status", "Cambiar estado", "Marcas"),

            ("presentations.view", "Ver presentaciones", "Presentaciones"),
            ("presentations.create", "Agregar presentación", "Presentaciones"),
            ("presentations.edit", "Editar presentación", "Presentaciones"),
            ("presentations.delete", "Eliminar presentación", "Presentaciones"),
            ("presentations.status", "Cambiar estado", "Presentaciones"),

            ("suppliers.view", "Ver proveedores", "Proveedores"),
            ("suppliers.create", "Agregar proveedor", "Proveedores"),
            ("suppliers.edit", "Editar proveedor", "Proveedores"),
            ("suppliers.delete", "Eliminar proveedor", "Proveedores"),
            ("suppliers.status", "Cambiar estado", "Proveedores"),

            ("sales.view", "Ver ventas", "Ventas"),
            ("sales.create", "Agregar venta", "Ventas"),
            ("sales.cancel", "Anular venta", "Ventas"),
            ("sales.return", "Devolución", "Ventas"),
            ("sales.invoice", "Generar factura", "Ventas"),
            ("sales.export", "Exportar ventas", "Ventas"),

            ("services.view", "Ver servicios", "Servicios"),
            ("services.create", "Agregar servicio", "Servicios"),
            ("services.edit", "Editar servicio", "Servicios"),
            ("services.delete", "Eliminar servicio", "Servicios"),
            ("services.status", "Cambiar estado", "Servicios"),

            ("appointments.create", "Agregar cita", "Citas Médicas"),
            ("appointments.calendar", "Ver calendario", "Citas Médicas"),
            ("appointments.list", "Lista de citas", "Citas Médicas"),
            ("appointments.status", "Cambiar estado cita", "Citas Médicas"),
            ("appointments.availability", "Disponibilidad", "Citas Médicas"),
            ("appointments.doctors.view", "Ver médicos", "Citas Médicas"),
            ("appointments.doctors.create", "Agregar médico", "Citas Médicas"),
            ("appointments.doctors.edit", "Editar médico", "Citas Médicas"),
            ("appointments.doctors.delete", "Eliminar médico", "Citas Médicas"),
            ("appointments.doctors.status", "Cambiar estado médico", "Citas Médicas"),

            ("reports.shifts", "Historial de Turnos", "Reportes"),
            ("reports.performance", "Desempeño de Empleados", "Reportes"),

            ("system.roles", "Gestionar Roles", "Configuración"),
            ("config.service_categories.create", "Agregar categoría de servicio", "Configuración"),
            ("config.service_categories.edit", "Editar categoría de servicio", "Configuración"),
            ("config.service_categories.delete", "Eliminar categoría de servicio", "Configuración"),
            ("config.payment_methods.create", "Agregar método de pago", "Configuración"),
            ("config.payment_methods.edit", "Editar método de pago", "Configuración"),
            ("config.payment_methods.delete", "Eliminar método de pago", "Configuración"),
            ("config.document_types.create", "Agregar tipo de documento", "Configuración"),
            ("config.document_types.edit", "Editar tipo de documento", "Configuración"),
            ("config.document_types.delete", "Eliminar tipo de documento", "Configuración"),
        };

        private static async Task SeedPermisosAsync(SyspharmaContext context, Role roleAdmin)
        {
            // Inserta solo los códigos del catálogo que todavía no existan — permite
            // agregar permisos nuevos en despliegues futuros sin tocar los ya creados.
            var codigosExistentes = await context.Permisos.Select(p => p.Codigo).ToListAsync();
            var faltantes = PermisosCatalogo.Where(p => !codigosExistentes.Contains(p.Codigo)).ToList();
            if (faltantes.Count > 0)
            {
                context.Permisos.AddRange(faltantes.Select(p => new Permiso
                {
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria,
                    FechaCreacion = DateTime.Now
                }));
                await context.SaveChangesAsync();
            }

            // El rol Administrador tiene bypass de permisos en RequirePermissionFilter,
            // pero se le asignan todos igual para que la pantalla de Configuración > Roles
            // refleje el estado real (checkboxes marcados) en vez de verse vacía.
            var permisoIdsAsignados = await context.RolesPermisos
                .Where(rp => rp.RoleId == roleAdmin.Id)
                .Select(rp => rp.PermisoId)
                .ToListAsync();
            var permisoIdsFaltantes = await context.Permisos
                .Where(p => !permisoIdsAsignados.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();
            if (permisoIdsFaltantes.Count > 0)
            {
                context.RolesPermisos.AddRange(permisoIdsFaltantes.Select(id => new RolesPermiso
                {
                    RoleId = roleAdmin.Id,
                    PermisoId = id,
                    FechaAsignacion = DateTime.Now
                }));
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<Usuario> userManager, SyspharmaContext context, Role roleAdmin)
        {
            if (await context.Usuarios.AnyAsync()) return;

            // Cambiar esta contraseña inmediatamente después del primer login.
            var email = "admin@syspharma.local";
            var password = "XOnDvEJSfAsb+J7P";

            var admin = new Usuario
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nombre = "Administrador",
                RoleId = roleAdmin.Id,
                Estado = true,
                FechaCreacion = DateTime.Now
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                var errores = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"No se pudo crear el usuario Administrador inicial: {errores}");
            }
        }
    }
}
