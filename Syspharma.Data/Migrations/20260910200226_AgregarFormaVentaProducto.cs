using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFormaVentaProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "factorUnidades",
                table: "venta_detalles",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "formaVentaId",
                table: "venta_detalles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "formaVentaTipo",
                table: "venta_detalles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "factorUnidades",
                table: "pedido_detalles",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "formaVentaId",
                table: "pedido_detalles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "formaVentaTipo",
                table: "pedido_detalles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "formaVentaId",
                table: "carrito_items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "producto_forma_venta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productoId = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    precio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    factorUnidades = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_forma_venta", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProductoFormaVenta_Productos",
                        column: x => x.productoId,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalles_formaVentaId",
                table: "venta_detalles",
                column: "formaVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalles_formaVentaId",
                table: "pedido_detalles",
                column: "formaVentaId");

            migrationBuilder.CreateIndex(
                name: "UQ_producto_forma_venta_producto_tipo",
                table: "producto_forma_venta",
                columns: new[] { "productoId", "tipo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_ProductoFormaVenta",
                table: "pedido_detalles",
                column: "formaVentaId",
                principalTable: "producto_forma_venta",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VentaDetalles_ProductoFormaVenta",
                table: "venta_detalles",
                column: "formaVentaId",
                principalTable: "producto_forma_venta",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // Backfill: todo producto existente sigue vendiéndose igual que hoy (forma "Unidad",
            // factor 1, precio = precio actual) hasta que un admin habilite Blister/Caja.
            migrationBuilder.Sql(@"
                INSERT INTO producto_forma_venta (productoId, tipo, precio, factorUnidades, activo)
                SELECT id, 'Unidad', precio, 1, 1 FROM productos;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_ProductoFormaVenta",
                table: "pedido_detalles");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaDetalles_ProductoFormaVenta",
                table: "venta_detalles");

            migrationBuilder.DropTable(
                name: "producto_forma_venta");

            migrationBuilder.DropIndex(
                name: "IX_venta_detalles_formaVentaId",
                table: "venta_detalles");

            migrationBuilder.DropIndex(
                name: "IX_pedido_detalles_formaVentaId",
                table: "pedido_detalles");

            migrationBuilder.DropColumn(
                name: "factorUnidades",
                table: "venta_detalles");

            migrationBuilder.DropColumn(
                name: "formaVentaId",
                table: "venta_detalles");

            migrationBuilder.DropColumn(
                name: "formaVentaTipo",
                table: "venta_detalles");

            migrationBuilder.DropColumn(
                name: "factorUnidades",
                table: "pedido_detalles");

            migrationBuilder.DropColumn(
                name: "formaVentaId",
                table: "pedido_detalles");

            migrationBuilder.DropColumn(
                name: "formaVentaTipo",
                table: "pedido_detalles");

            migrationBuilder.DropColumn(
                name: "formaVentaId",
                table: "carrito_items");
        }
    }
}
