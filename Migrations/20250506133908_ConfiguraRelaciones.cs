using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguraRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jugadores_Equipos_EquipoLocalId",
                table: "Jugadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Jugadores_Equipos_EquipoVisitanteId",
                table: "Jugadores");

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

            migrationBuilder.RenameColumn(
                name: "Jugadores",
                table: "Equipos",
                newName: "JugadoresEnBanca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JugadoresEnBanca",
                table: "Equipos",
                newName: "Jugadores");

            migrationBuilder.AddColumn<int>(
                name: "EquipoLocalId",
                table: "Jugadores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipoVisitanteId",
                table: "Jugadores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Jugadores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GolesVisitante",
                table: "Jugadores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_EquipoLocalId",
                table: "Jugadores",
                column: "EquipoLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_EquipoVisitanteId",
                table: "Jugadores",
                column: "EquipoVisitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jugadores_Equipos_EquipoLocalId",
                table: "Jugadores",
                column: "EquipoLocalId",
                principalTable: "Equipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jugadores_Equipos_EquipoVisitanteId",
                table: "Jugadores",
                column: "EquipoVisitanteId",
                principalTable: "Equipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
