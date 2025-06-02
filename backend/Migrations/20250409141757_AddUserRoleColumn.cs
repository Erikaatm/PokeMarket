using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeMarket.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Gradings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    GradedById = table.Column<int>(type: "int", nullable: false),
                    GradedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gradings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gradings_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gradings_Users_GradedById",
                        column: x => x.GradedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_CardId",
                table: "Favorites",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Gradings_CardId",
                table: "Gradings",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Gradings_GradedById",
                table: "Gradings",
                column: "GradedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Cards_CardId",
                table: "Favorites",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Cards_CardId",
                table: "Favorites");

            migrationBuilder.DropTable(
                name: "Gradings");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_CardId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
