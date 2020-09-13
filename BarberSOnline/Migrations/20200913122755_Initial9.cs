using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations
{
    public partial class Initial9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Appointment_Status",
                table: "AppointmentModel",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AddColumn<string>(
                name: "BarberEmail",
                table: "AppointmentModel",
                maxLength: 60,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberEmail",
                table: "AppointmentModel");

            migrationBuilder.AlterColumn<string>(
                name: "Appointment_Status",
                table: "AppointmentModel",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
