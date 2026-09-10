using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaMarcas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "marca",
                table: "productos");

            migrationBuilder.AddColumn<int>(
                name: "marcaId",
                table: "productos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "marcas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productos_marcaId",
                table: "productos",
                column: "marcaId");

            migrationBuilder.CreateIndex(
                name: "IX_marcas_nombre",
                table: "marcas",
                column: "nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Marca",
                table: "productos",
                column: "marcaId",
                principalTable: "marcas",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Marca",
                table: "productos");

            migrationBuilder.DropTable(
                name: "marcas");

            migrationBuilder.DropIndex(
                name: "IX_productos_marcaId",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "marcaId",
                table: "productos");

            migrationBuilder.AddColumn<string>(
                name: "marca",
                table: "productos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
