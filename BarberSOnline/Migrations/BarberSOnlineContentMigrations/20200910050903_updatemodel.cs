using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations.BarberSOnlineContentMigrations
{
    public partial class updatemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature",
                table: "UserModel",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Temperature",
                table: "UserModel",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
