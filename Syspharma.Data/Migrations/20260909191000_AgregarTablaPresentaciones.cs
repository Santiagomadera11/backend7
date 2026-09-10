using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaPresentaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "presentacion",
                table: "productos");

            migrationBuilder.AddColumn<int>(
                name: "presentacionId",
                table: "productos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "presentaciones",
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
                    table.PrimaryKey("PK_presentaciones", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productos_presentacionId",
                table: "productos",
                column: "presentacionId");

            migrationBuilder.CreateIndex(
                name: "IX_presentaciones_nombre",
                table: "presentaciones",
                column: "nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Presentacion",
                table: "productos",
                column: "presentacionId",
                principalTable: "presentaciones",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Presentacion",
                table: "productos");

            migrationBuilder.DropTable(
                name: "presentaciones");

            migrationBuilder.DropIndex(
                name: "IX_productos_presentacionId",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "presentacionId",
                table: "productos");

            migrationBuilder.AddColumn<string>(
                name: "presentacion",
                table: "productos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
