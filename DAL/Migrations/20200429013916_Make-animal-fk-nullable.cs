using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Makeanimalfknullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredAnimals_Animals_AnimalId",
                table: "RegisteredAnimals");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnimalId",
                table: "RegisteredAnimals",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredAnimals_Animals_AnimalId",
                table: "RegisteredAnimals",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredAnimals_Animals_AnimalId",
                table: "RegisteredAnimals");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnimalId",
                table: "RegisteredAnimals",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredAnimals_Animals_AnimalId",
                table: "RegisteredAnimals",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
