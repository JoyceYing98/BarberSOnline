using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations
{
    public partial class Intial10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barber_Confirmed_Date",
                table: "AppointmentModel");

            migrationBuilder.AddColumn<DateTime>(
                name: "Barber_Approved_Date",
                table: "AppointmentModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barber_Approved_Date",
                table: "AppointmentModel");

            migrationBuilder.AddColumn<DateTime>(
                name: "Barber_Confirmed_Date",
                table: "AppointmentModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
