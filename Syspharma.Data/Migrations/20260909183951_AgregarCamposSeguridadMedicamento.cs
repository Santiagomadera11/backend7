using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposSeguridadMedicamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "afectaConduccion",
                table: "producto_medicamento",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "fotosensible",
                table: "producto_medicamento",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "indicaciones",
                table: "producto_medicamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "posologia",
                table: "producto_medicamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "requiereRefrigeracion",
                table: "producto_medicamento",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "unidadesPorEnvase",
                table: "producto_medicamento",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "afectaConduccion",
                table: "producto_medicamento");

            migrationBuilder.DropColumn(
                name: "fotosensible",
                table: "producto_medicamento");

            migrationBuilder.DropColumn(
                name: "indicaciones",
                table: "producto_medicamento");

            migrationBuilder.DropColumn(
                name: "posologia",
                table: "producto_medicamento");

            migrationBuilder.DropColumn(
                name: "requiereRefrigeracion",
                table: "producto_medicamento");

            migrationBuilder.DropColumn(
                name: "unidadesPorEnvase",
                table: "producto_medicamento");
        }
    }
}
