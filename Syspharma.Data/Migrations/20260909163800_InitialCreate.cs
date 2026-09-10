using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categoria_servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__3213E83F83A344EC", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__3213E83F3635F0B3", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    valor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_cita",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__estados___3213E83FD50846F6", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__estados___3213E83F596D49DE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_pedido",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__estados___3213E83FD81DA3EB", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_proveedor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__estados___3213E83FCFECEA39", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estados_venta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__estados___3213E83FDCF557D9", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosDevoluciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EstadosDevoluciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    diasLaborales = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    horaInicio = table.Column<TimeOnly>(type: "time", nullable: true),
                    horaFin = table.Column<TimeOnly>(type: "time", nullable: true),
                    intervalo = table.Column<int>(type: "int", nullable: true, defaultValue: 30),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__medicos__3213E83F08A4C5C1", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "metodos_pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__metodos___3213E83FBFEBC96F", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__roles__3213E83F06400409", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_documento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tipos_do__3213E83FCA8B537B", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VVentaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    ProductoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubtotalCalculado = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VVentaDetalles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    categoriaId = table.Column<int>(type: "int", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    duracion = table.Column<int>(type: "int", nullable: true),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__servicio__3213E83F9123C840", x => x.id);
                    table.ForeignKey(
                        name: "FK_Servicios_Categoria",
                        column: x => x.categoriaId,
                        principalTable: "categoria_servicios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "disponibilidad_bloqueos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    medicoId = table.Column<int>(type: "int", nullable: false),
                    fechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    motivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disponibilidad_bloqueos", x => x.id);
                    table.ForeignKey(
                        name: "FK_DisponibilidadBloqueos_Medicos",
                        column: x => x.medicoId,
                        principalTable: "medicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "disponibilidad_horarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    medicoId = table.Column<int>(type: "int", nullable: false),
                    diaSemana = table.Column<int>(type: "int", nullable: false),
                    mananaInicio = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    mananaFin = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    tardeInicio = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    tardeFin = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disponibilidad_horarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_DisponibilidadHorarios_Medicos",
                        column: x => x.medicoId,
                        principalTable: "medicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicoDiaNoDisponible",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicoDiaNoDisponible", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicoDiaNoDisponible_medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "medicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicoHorario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<byte>(type: "tinyint", nullable: false),
                    MananaInicio = table.Column<TimeOnly>(type: "time", nullable: true),
                    MananaFin = table.Column<TimeOnly>(type: "time", nullable: true),
                    TardeInicio = table.Column<TimeOnly>(type: "time", nullable: true),
                    TardeFin = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicoHorario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicoHorario_medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "medicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roles_permisos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    permisoId = table.Column<int>(type: "int", nullable: false),
                    fechaAsignacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__roles_pe__3213E83F567DFDB5", x => x.id);
                    table.ForeignKey(
                        name: "FK_RolesPermisos_Permisos",
                        column: x => x.permisoId,
                        principalTable: "Permisos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolesPermisos_Roles",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "proveedores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    contacto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipoDocumentoId = table.Column<int>(type: "int", nullable: true),
                    documento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    estadoId = table.Column<int>(type: "int", nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__proveedo__3213E83FFD96C03C", x => x.id);
                    table.ForeignKey(
                        name: "FK_Proveedores_Estado",
                        column: x => x.estadoId,
                        principalTable: "estados_proveedor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Proveedores_TipoDocumento",
                        column: x => x.tipoDocumentoId,
                        principalTable: "tipos_documento",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    tipoDocumentoId = table.Column<int>(type: "int", nullable: true),
                    documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ultimoAcceso = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Usuarios_TipoDocumento",
                        column: x => x.tipoDocumentoId,
                        principalTable: "tipos_documento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    categoriaId = table.Column<int>(type: "int", nullable: false),
                    proveedorId = table.Column<int>(type: "int", nullable: true),
                    precio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    precioCompra = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    presentacion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    stock = table.Column<int>(type: "int", nullable: false),
                    codigoBarras = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    imagen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ultimaActualizacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fechaVencimientoProxima = table.Column<DateOnly>(type: "date", nullable: true),
                    porcentajeIva = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0.00m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__producto__3213E83F11CBC83A", x => x.id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias",
                        column: x => x.categoriaId,
                        principalTable: "categorias",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores",
                        column: x => x.proveedorId,
                        principalTable: "proveedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "compras",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numeroCompra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    proveedorId = table.Column<int>(type: "int", nullable: false),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    estadoId = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    iva = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaCompra = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fechaEntrega = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__compras__3213E83F8314C9C0", x => x.id);
                    table.ForeignKey(
                        name: "FK_Compras_Estado",
                        column: x => x.estadoId,
                        principalTable: "estados_compra",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Compras_Proveedores",
                        column: x => x.proveedorId,
                        principalTable: "proveedores",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Compras_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numeroPedido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    usuarioId = table.Column<int>(type: "int", nullable: true),
                    clienteNombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    clienteDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    clienteTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    clienteEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    metodoPagoId = table.Column<int>(type: "int", nullable: true),
                    estadoId = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    iva = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    origen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "web"),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fechaEntrega = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__pedidos__3213E83F5DD7EBD1", x => x.id);
                    table.ForeignKey(
                        name: "FK_Pedidos_Estado",
                        column: x => x.estadoId,
                        principalTable: "estados_pedido",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Pedidos_MetodoPago",
                        column: x => x.metodoPagoId,
                        principalTable: "metodos_pago",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "turnos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "activo"),
                    montoBase = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    montoFinal = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    totalVentas = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    totalGastos = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    resumenVentas = table.Column<int>(type: "int", nullable: false),
                    resumenServicios = table.Column<int>(type: "int", nullable: false),
                    resumenErroresCaja = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    diferencia = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaApertura = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fechaCierre = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__turnos__3213E83F9A3D7F5A", x => x.id);
                    table.ForeignKey(
                        name: "FK_Turnos_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "producto_medicamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    composicion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concentracion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viaAdministracion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    registroSanitario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requiereFormula = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_medicamento", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProductoMedicamento_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "compra_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    compraId = table.Column<int>(type: "int", nullable: false),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    lote = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaVencimiento = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__compra_d__3213E83F3E646B3C", x => x.id);
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Compras",
                        column: x => x.compraId,
                        principalTable: "compras",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    compraId = table.Column<int>(type: "int", nullable: true),
                    numeroLote = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    fechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    costoUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lotes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Lotes_Compras",
                        column: x => x.compraId,
                        principalTable: "compras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Lotes_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedido_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedidoId = table.Column<int>(type: "int", nullable: false),
                    productoId = table.Column<int>(type: "int", nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__pedido_d__3213E83F48F2B407", x => x.id);
                    table.ForeignKey(
                        name: "FK_PedidoDetalles_Pedidos",
                        column: x => x.pedidoId,
                        principalTable: "pedidos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_PedidoDetalles_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "gastos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    turnoId = table.Column<int>(type: "int", nullable: false),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    monto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "operacional"),
                    comprobante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaGasto = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    anulado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    fechaAnulacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    motivoAnulacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__gastos__3213E83F9BA914EE", x => x.id);
                    table.ForeignKey(
                        name: "FK_Gastos_Turnos",
                        column: x => x.turnoId,
                        principalTable: "turnos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Gastos_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numeroVenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    turnoId = table.Column<int>(type: "int", nullable: false),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    clienteNombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    clienteDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    clienteTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    metodoPagoId = table.Column<int>(type: "int", nullable: false),
                    estadoId = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    iva = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    porcentajeIva = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaVenta = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    origen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "CAJA"),
                    pedidoId = table.Column<int>(type: "int", nullable: true),
                    referenciasPago = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ventas__3213E83F3E2DA856", x => x.id);
                    table.ForeignKey(
                        name: "FK_Ventas_Estado",
                        column: x => x.estadoId,
                        principalTable: "estados_venta",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Ventas_MetodoPago",
                        column: x => x.metodoPagoId,
                        principalTable: "metodos_pago",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Ventas_Pedidos",
                        column: x => x.pedidoId,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ventas_Turnos",
                        column: x => x.turnoId,
                        principalTable: "turnos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    medicoId = table.Column<int>(type: "int", nullable: false),
                    pacienteNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    pacienteDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    pacienteTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    pacienteEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    servicioNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    precio = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    estadoId = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    servicioId = table.Column<int>(type: "int", nullable: true),
                    usuarioId = table.Column<int>(type: "int", nullable: true),
                    pedidoId = table.Column<int>(type: "int", nullable: true),
                    ventaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__citas__3213E83FC793410B", x => x.id);
                    table.ForeignKey(
                        name: "FK_Citas_Estado",
                        column: x => x.estadoId,
                        principalTable: "estados_cita",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Citas_Medicos",
                        column: x => x.medicoId,
                        principalTable: "medicos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Citas_Pedidos",
                        column: x => x.pedidoId,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Citas_Servicios",
                        column: x => x.servicioId,
                        principalTable: "servicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Citas_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Citas_Ventas",
                        column: x => x.ventaId,
                        principalTable: "ventas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Devoluciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ventaId = table.Column<int>(type: "int", nullable: false),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    estadoId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    totalDevolucion = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    fechaDevolucion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    fechaGestion = table.Column<DateTime>(type: "datetime", nullable: true),
                    usuarioGestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Devoluciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Dev_Estado",
                        column: x => x.estadoId,
                        principalTable: "EstadosDevoluciones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Dev_Usuario",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Dev_Venta",
                        column: x => x.ventaId,
                        principalTable: "ventas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "venta_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ventaId = table.Column<int>(type: "int", nullable: false),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    loteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__venta_de__3213E83FD3C3548D", x => x.id);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Lotes",
                        column: x => x.loteId,
                        principalTable: "lotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Ventas",
                        column: x => x.ventaId,
                        principalTable: "ventas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "venta_detalles_servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ventaId = table.Column<int>(type: "int", nullable: false),
                    servicioId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    citaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__venta_detalles_servicios", x => x.id);
                    table.ForeignKey(
                        name: "FK_VentaDetalleServicios_Citas",
                        column: x => x.citaId,
                        principalTable: "citas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VentaDetalleServicios_Servicios",
                        column: x => x.servicioId,
                        principalTable: "servicios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_VentaDetalleServicios_Ventas",
                        column: x => x.ventaId,
                        principalTable: "ventas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "DetallesDevoluciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    devolucionId = table.Column<int>(type: "int", nullable: false),
                    detalleVentaId = table.Column<int>(type: "int", nullable: false),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    cantidadDevuelta = table.Column<int>(type: "int", nullable: false),
                    precioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    subtotalDevuelto = table.Column<decimal>(type: "decimal(12,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetallesDevoluciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_DetDev_DetalleVenta",
                        column: x => x.detalleVentaId,
                        principalTable: "venta_detalles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_DetDev_Devolucion",
                        column: x => x.devolucionId,
                        principalTable: "Devoluciones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_DetDev_Producto",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__categori__72AFBCC6A71D1D12",
                table: "categoria_servicios",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__categori__72AFBCC6D44E580C",
                table: "categorias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_citas_medico_fecha",
                table: "citas",
                columns: new[] { "medicoId", "fecha" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_citas_estadoId",
                table: "citas",
                column: "estadoId");

            migrationBuilder.CreateIndex(
                name: "IX_citas_pedidoId",
                table: "citas",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_citas_servicioId",
                table: "citas",
                column: "servicioId");

            migrationBuilder.CreateIndex(
                name: "IX_citas_usuarioId",
                table: "citas",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_citas_ventaId",
                table: "citas",
                column: "ventaId");

            migrationBuilder.CreateIndex(
                name: "IX_compra_detalles_compraId",
                table: "compra_detalles",
                column: "compraId");

            migrationBuilder.CreateIndex(
                name: "IX_compra_detalles_productoId",
                table: "compra_detalles",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "idx_compras_proveedor",
                table: "compras",
                column: "proveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_compras_estadoId",
                table: "compras",
                column: "estadoId");

            migrationBuilder.CreateIndex(
                name: "IX_compras_usuarioId",
                table: "compras",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "UQ__compras__6EB8ED51AA2AF7FC",
                table: "compras",
                column: "numeroCompra",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_configuracion_clave",
                table: "configuracion",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDevoluciones_detalleVentaId",
                table: "DetallesDevoluciones",
                column: "detalleVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDevoluciones_devolucionId",
                table: "DetallesDevoluciones",
                column: "devolucionId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDevoluciones_productoId",
                table: "DetallesDevoluciones",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_estadoId",
                table: "Devoluciones",
                column: "estadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_usuarioId",
                table: "Devoluciones",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_ventaId",
                table: "Devoluciones",
                column: "ventaId");

            migrationBuilder.CreateIndex(
                name: "IX_disponibilidad_bloqueos_medicoId",
                table: "disponibilidad_bloqueos",
                column: "medicoId");

            migrationBuilder.CreateIndex(
                name: "IX_disponibilidad_horarios_medicoId",
                table: "disponibilidad_horarios",
                column: "medicoId");

            migrationBuilder.CreateIndex(
                name: "UQ__estados___72AFBCC60D8E2BC0",
                table: "estados_cita",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__estados___72AFBCC605F19684",
                table: "estados_compra",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__estados___72AFBCC6CC1257F9",
                table: "estados_pedido",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__estados___72AFBCC66CC17226",
                table: "estados_proveedor",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__estados___72AFBCC61A2305EF",
                table: "estados_venta",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gastos_turnoId",
                table: "gastos",
                column: "turnoId");

            migrationBuilder.CreateIndex(
                name: "IX_gastos_usuarioId",
                table: "gastos",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_lotes_compraId",
                table: "lotes",
                column: "compraId");

            migrationBuilder.CreateIndex(
                name: "IX_lotes_productoId",
                table: "lotes",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicoDiaNoDisponible_MedicoId",
                table: "MedicoDiaNoDisponible",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicoHorario_MedicoId_DiaSemana",
                table: "MedicoHorario",
                columns: new[] { "MedicoId", "DiaSemana" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__metodos___72AFBCC64CAE685E",
                table: "metodos_pago",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalles_pedidoId",
                table: "pedido_detalles",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalles_productoId",
                table: "pedido_detalles",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "idx_pedidos_estado",
                table: "pedidos",
                column: "estadoId");

            migrationBuilder.CreateIndex(
                name: "idx_pedidos_fecha",
                table: "pedidos",
                column: "fechaCreacion",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_metodoPagoId",
                table: "pedidos",
                column: "metodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_usuarioId",
                table: "pedidos",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "UQ__pedidos__90DD6149AD9BA5B6",
                table: "pedidos",
                column: "numeroPedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_producto_medicamento_productoId",
                table: "producto_medicamento",
                column: "productoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_productos_categoria_estado",
                table: "productos",
                columns: new[] { "categoriaId", "estado" });

            migrationBuilder.CreateIndex(
                name: "idx_productos_nombre",
                table: "productos",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_productos_proveedorId",
                table: "productos",
                column: "proveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_proveedores_estadoId",
                table: "proveedores",
                column: "estadoId");

            migrationBuilder.CreateIndex(
                name: "IX_proveedores_tipoDocumentoId",
                table: "proveedores",
                column: "tipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "UQ__proveedo__72AFBCC64091315F",
                table: "proveedores",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__roles__72AFBCC689F96075",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_permisos_permisoId",
                table: "roles_permisos",
                column: "permisoId");

            migrationBuilder.CreateIndex(
                name: "UQ_Role_Permiso",
                table: "roles_permisos",
                columns: new[] { "roleId", "permisoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_servicios_categoria",
                table: "servicios",
                column: "categoriaId");

            migrationBuilder.CreateIndex(
                name: "idx_servicios_estado",
                table: "servicios",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "UQ__tipos_do__72AFBCC640703EE9",
                table: "tipos_documento",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_turnos_usuarioId",
                table: "turnos",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "usuarios",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_roleId",
                table: "usuarios",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_tipoDocumentoId",
                table: "usuarios",
                column: "tipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuarios",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_loteId",
                table: "venta_detalles",
                column: "loteId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_productoId",
                table: "venta_detalles",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_ventaId",
                table: "venta_detalles",
                column: "ventaId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_servicios_citaId",
                table: "venta_detalles_servicios",
                column: "citaId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_servicios_servicioId",
                table: "venta_detalles_servicios",
                column: "servicioId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_servicios_ventaId",
                table: "venta_detalles_servicios",
                column: "ventaId");

            migrationBuilder.CreateIndex(
                name: "idx_ventas_turno_fecha",
                table: "ventas",
                columns: new[] { "turnoId", "fechaVenta" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_ventas_estadoId",
                table: "ventas",
                column: "estadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_metodoPagoId",
                table: "ventas",
                column: "metodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_pedidoId",
                table: "ventas",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_usuarioId",
                table: "ventas",
                column: "usuarioId");

            migrationBuilder.CreateIndex(
                name: "UQ__ventas__44FDAC498B274DB5",
                table: "ventas",
                column: "numeroVenta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "compra_detalles");

            migrationBuilder.DropTable(
                name: "configuracion");

            migrationBuilder.DropTable(
                name: "DetallesDevoluciones");

            migrationBuilder.DropTable(
                name: "disponibilidad_bloqueos");

            migrationBuilder.DropTable(
                name: "disponibilidad_horarios");

            migrationBuilder.DropTable(
                name: "gastos");

            migrationBuilder.DropTable(
                name: "MedicoDiaNoDisponible");

            migrationBuilder.DropTable(
                name: "MedicoHorario");

            migrationBuilder.DropTable(
                name: "pedido_detalles");

            migrationBuilder.DropTable(
                name: "producto_medicamento");

            migrationBuilder.DropTable(
                name: "roles_permisos");

            migrationBuilder.DropTable(
                name: "venta_detalles_servicios");

            migrationBuilder.DropTable(
                name: "VVentaDetalles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "venta_detalles");

            migrationBuilder.DropTable(
                name: "Devoluciones");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "lotes");

            migrationBuilder.DropTable(
                name: "EstadosDevoluciones");

            migrationBuilder.DropTable(
                name: "estados_cita");

            migrationBuilder.DropTable(
                name: "medicos");

            migrationBuilder.DropTable(
                name: "servicios");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "compras");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "categoria_servicios");

            migrationBuilder.DropTable(
                name: "estados_venta");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "turnos");

            migrationBuilder.DropTable(
                name: "estados_compra");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "proveedores");

            migrationBuilder.DropTable(
                name: "estados_pedido");

            migrationBuilder.DropTable(
                name: "metodos_pago");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "estados_proveedor");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "tipos_documento");
        }
    }
}
