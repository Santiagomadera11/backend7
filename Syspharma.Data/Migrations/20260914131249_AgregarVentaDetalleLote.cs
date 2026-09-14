using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVentaDetalleLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "venta_detalle_lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ventaDetalleId = table.Column<int>(type: "int", nullable: false),
                    loteId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venta_detalle_lotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaDetalleLotes_Lotes",
                        column: x => x.loteId,
                        principalTable: "lotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentaDetalleLotes_VentaDetalles",
                        column: x => x.ventaDetalleId,
                        principalTable: "venta_detalles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalle_lotes_loteId",
                table: "venta_detalle_lotes",
                column: "loteId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_detalle_lotes_ventaDetalleId",
                table: "venta_detalle_lotes",
                column: "ventaDetalleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "venta_detalle_lotes");
        }
    }
}
