using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuitarPedidosCarritoNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Pedidos",
                table: "citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Pedidos",
                table: "ventas");

            migrationBuilder.DropTable(
                name: "carrito_items");

            migrationBuilder.DropTable(
                name: "notificaciones");

            migrationBuilder.DropTable(
                name: "pedido_detalles");

            migrationBuilder.DropTable(
                name: "carritos");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "estados_pedido");

            migrationBuilder.DropIndex(
                name: "IX_ventas_pedidoId",
                table: "ventas");

            migrationBuilder.DropIndex(
                name: "IX_citas_pedidoId",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "pedidoId",
                table: "ventas");

            migrationBuilder.DropColumn(
                name: "pedidoId",
                table: "citas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pedidoId",
                table: "ventas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "pedidoId",
                table: "citas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "carritos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    fechaActualizacion = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carritos", x => x.id);
                    table.ForeignKey(
                        name: "FK_Carritos_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "notificaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioId = table.Column<int>(type: "int", nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    leida = table.Column<bool>(type: "bit", nullable: false),
                    mensaje = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios",
                        column: x => x.usuarioId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carrito_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carritoId = table.Column<int>(type: "int", nullable: false),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    formaVentaId = table.Column<int>(type: "int", nullable: true),
                    precioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carrito_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Carritos",
                        column: x => x.carritoId,
                        principalTable: "carritos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    estadoId = table.Column<int>(type: "int", nullable: false),
                    metodoPagoId = table.Column<int>(type: "int", nullable: true),
                    usuarioId = table.Column<int>(type: "int", nullable: true),
                    clienteDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    clienteEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    clienteNombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    clienteTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fechaEntrega = table.Column<DateTime>(type: "datetime", nullable: true),
                    iva = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numeroPedido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    origen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "web"),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
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
                name: "pedido_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    formaVentaId = table.Column<int>(type: "int", nullable: true),
                    pedidoId = table.Column<int>(type: "int", nullable: false),
                    productoId = table.Column<int>(type: "int", nullable: true),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    factorUnidades = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    formaVentaTipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                        name: "FK_PedidoDetalles_ProductoFormaVenta",
                        column: x => x.formaVentaId,
                        principalTable: "producto_forma_venta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalles_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ventas_pedidoId",
                table: "ventas",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_citas_pedidoId",
                table: "citas",
                column: "pedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_carrito_items_carritoId",
                table: "carrito_items",
                column: "carritoId");

            migrationBuilder.CreateIndex(
                name: "IX_carrito_items_productoId",
                table: "carrito_items",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_carritos_usuarioId",
                table: "carritos",
                column: "usuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__estados___72AFBCC6CC1257F9",
                table: "estados_pedido",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_usuarioId_fechaCreacion",
                table: "notificaciones",
                columns: new[] { "usuarioId", "fechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalles_formaVentaId",
                table: "pedido_detalles",
                column: "formaVentaId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Pedidos",
                table: "citas",
                column: "pedidoId",
                principalTable: "pedidos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Pedidos",
                table: "ventas",
                column: "pedidoId",
                principalTable: "pedidos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
