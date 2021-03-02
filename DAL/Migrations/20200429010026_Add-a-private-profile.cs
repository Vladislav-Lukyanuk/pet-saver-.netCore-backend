using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Addaprivateprofile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredAnimals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    QR = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UploadDate = table.Column<DateTimeOffset>(nullable: false),
                    AnimalId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredAnimals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisteredAnimals_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegisteredAnimals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredAnimals_AnimalId",
                table: "RegisteredAnimals",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredAnimals_UserId",
                table: "RegisteredAnimals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisteredAnimals");
        }
    }
}
