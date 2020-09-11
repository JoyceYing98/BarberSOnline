using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "AppointmentModel",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "ShopEmail",
                table: "AppointmentModel",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "AdminEmail",
                table: "AppointmentModel",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 60);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserEmail",
                table: "AppointmentModel",
                type: "int",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShopEmail",
                table: "AppointmentModel",
                type: "int",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AdminEmail",
                table: "AppointmentModel",
                type: "int",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60,
                oldNullable: true);
        }
    }
}
