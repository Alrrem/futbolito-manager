using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    public partial class CrearTablaCanchas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Canchas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canchas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Canchas");
        }
    }
}
