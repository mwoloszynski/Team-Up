using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamUp.Migrations
{
    public partial class LocalizationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "latitude",
                table: "Teams",
                type: "decimal(9,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "longtitude",
                table: "Teams",
                type: "decimal(9,9)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "longtitude",
                table: "Teams");
        }
    }
}
