using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syspharma.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPorcentajeIvaProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "porcentajeIva",
                table: "productos",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "porcentajeIva",
                table: "productos");
        }
    }
}
