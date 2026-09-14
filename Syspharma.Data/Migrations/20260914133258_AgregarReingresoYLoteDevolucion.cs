using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReingresoYLoteDevolucion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "reingresa",
                table: "DetallesDevoluciones",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "detalle_devolucion_lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    detalleDevolucionId = table.Column<int>(type: "int", nullable: false),
                    loteId = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_devolucion_lotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionLotes_DetallesDevolucion",
                        column: x => x.detalleDevolucionId,
                        principalTable: "DetallesDevoluciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionLotes_Lotes",
                        column: x => x.loteId,
                        principalTable: "lotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detalle_devolucion_lotes_detalleDevolucionId",
                table: "detalle_devolucion_lotes",
                column: "detalleDevolucionId");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_devolucion_lotes_loteId",
                table: "detalle_devolucion_lotes",
                column: "loteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_devolucion_lotes");

            migrationBuilder.DropColumn(
                name: "reingresa",
                table: "DetallesDevoluciones");
        }
    }
}
