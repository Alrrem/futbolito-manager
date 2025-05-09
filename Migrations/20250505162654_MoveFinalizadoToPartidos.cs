using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    public partial class MoveFinalizadoToPartidos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Quitamos de Jugadores (si aún existe ahí)
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.DropColumn(
                    name: "Finalizado",
                    table: "Jugadores");
            }

            // 2) Lo agregamos a Partidos
            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Partidos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1) Lo quitamos de Partidos
            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Partidos");

            // 2) (Opcional) lo volvemos a poner en Jugadores
            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Jugadores",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
