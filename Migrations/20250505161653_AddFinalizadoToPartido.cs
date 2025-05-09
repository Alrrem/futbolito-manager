using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalizadoToPartido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Partidos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Partidos");
        }
    }
}
