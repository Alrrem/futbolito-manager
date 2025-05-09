using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    public partial class AgregaCanchaAPartido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Agregar columna como nullable
            migrationBuilder.AddColumn<int>(
                name: "CanchaId",
                table: "Partidos",
                type: "int",
                nullable: true);

            // 2) Poblamos todos los partidos existentes con la primera cancha disponible
            migrationBuilder.Sql(@"
                UPDATE Partidos
                SET CanchaId = (SELECT TOP 1 Id FROM Canchas ORDER BY Id)
                WHERE CanchaId IS NULL;
            ");

            // 3) Convertir columna a non-nullable ahora que ya todos tienen valor
            migrationBuilder.AlterColumn<int>(
                name: "CanchaId",
                table: "Partidos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 4) Crear índice y FK
            migrationBuilder.CreateIndex(
                name: "IX_Partidos_CanchaId",
                table: "Partidos",
                column: "CanchaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Canchas_CanchaId",
                table: "Partidos",
                column: "CanchaId",
                principalTable: "Canchas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Canchas_CanchaId",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_Partidos_CanchaId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "CanchaId",
                table: "Partidos");
        }
    }
}
