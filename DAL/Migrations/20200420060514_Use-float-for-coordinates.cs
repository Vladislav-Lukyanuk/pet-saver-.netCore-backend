using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Usefloatforcoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Coordinates",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Coordinates",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Longitude",
                table: "Coordinates",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<short>(
                name: "Latitude",
                table: "Coordinates",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
