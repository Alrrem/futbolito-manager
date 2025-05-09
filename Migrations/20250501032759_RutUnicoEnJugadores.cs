using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    public partial class RutUnicoEnJugadores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Si ya existe un índice antiguo, lo eliminamos
            migrationBuilder.DropIndex(
                name: "IX_Jugadores_Rut",
                table: "Jugadores");

            // 2) Alteramos la columna Rut a nvarchar(12)
            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Jugadores",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // 3) Alteramos otras columnas según la definición actual del modelo
            migrationBuilder.AlterColumn<string>(
                name: "Posicion",
                table: "Jugadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Jugadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // 4) Creamos el índice único sobre Rut
            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_Rut",
                table: "Jugadores",
                column: "Rut",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1) Quitamos el índice único
            migrationBuilder.DropIndex(
                name: "IX_Jugadores_Rut",
                table: "Jugadores");

            // 2) Revertimos los cambios de columna
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Jugadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Posicion",
                table: "Jugadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Jugadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            // (Si deseas, podrías recrear el índice en Down, 
            //  pero normalmente no lo hacemos porque Up ya lo creó)
        }
    }
}
