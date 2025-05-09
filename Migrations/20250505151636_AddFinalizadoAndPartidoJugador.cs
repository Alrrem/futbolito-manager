using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    public partial class AddFinalizadoAndPartidoJugador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Corregimos las columnas: ahora son nullable
            migrationBuilder.AddColumn<int>(
                name: "EquipoLocalId",
                table: "Jugadores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipoVisitanteId",
                table: "Jugadores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Jugadores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Jugadores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GolesLocal",
                table: "Jugadores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GolesVisitante",
                table: "Jugadores",
                type: "int",
                nullable: true);

            // Tabla intermedia para goles por jugador en un partido
            migrationBuilder.CreateTable(
                name: "PartidoJugadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                             .Annotation("SqlServer:Identity", "1, 1"),
                    PartidoId = table.Column<int>(type: "int", nullable: false),
                    JugadorId = table.Column<int>(type: "int", nullable: false),
                    Goles = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidoJugadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidoJugadores_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartidoJugadores_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // ⚠️ importante para evitar cascada múltiple
                });

            // Índices
            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_EquipoLocalId",
                table: "Jugadores",
                column: "EquipoLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_EquipoVisitanteId",
                table: "Jugadores",
                column: "EquipoVisitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidoJugadores_JugadorId",
                table: "PartidoJugadores",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidoJugadores_PartidoId",
                table: "PartidoJugadores",
                column: "PartidoId");

            // Claves foráneas
            migrationBuilder.AddForeignKey(
                name: "FK_Jugadores_Equipos_EquipoLocalId",
                table: "Jugadores",
                column: "EquipoLocalId",
                principalTable: "Equipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jugadores_Equipos_EquipoVisitanteId",
                table: "Jugadores",
                column: "EquipoVisitanteId",
                principalTable: "Equipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jugadores_Equipos_EquipoLocalId",
                table: "Jugadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Jugadores_Equipos_EquipoVisitanteId",
                table: "Jugadores");

            migrationBuilder.DropTable(
                name: "PartidoJugadores");

            migrationBuilder.DropIndex(
                name: "IX_Jugadores_EquipoLocalId",
                table: "Jugadores");

            migrationBuilder.DropIndex(
                name: "IX_Jugadores_EquipoVisitanteId",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "EquipoLocalId",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "EquipoVisitanteId",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "GolesLocal",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "GolesVisitante",
                table: "Jugadores");
        }
    }
}
