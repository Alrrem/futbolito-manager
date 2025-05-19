using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolitoManager.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablasScrum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SprintRetrospectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SprintId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoQueFuncionó = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoQueNoFuncionó = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccionesMejora = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintRetrospectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintRetrospectives_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SprintId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComentariosPO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aprobado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintReviews_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SprintRetrospectives_SprintId",
                table: "SprintRetrospectives",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintReviews_SprintId",
                table: "SprintReviews",
                column: "SprintId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SprintRetrospectives");

            migrationBuilder.DropTable(
                name: "SprintReviews");
        }
    }
}
