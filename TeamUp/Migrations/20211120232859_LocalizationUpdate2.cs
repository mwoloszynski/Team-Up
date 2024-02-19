using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamUp.Migrations
{
    public partial class LocalizationUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "longtitude",
                table: "Teams",
                type: "decimal(9,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "Teams",
                type: "decimal(9,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,9)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "longtitude",
                table: "Teams",
                type: "decimal(9,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "Teams",
                type: "decimal(9,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)");
        }
    }
}
