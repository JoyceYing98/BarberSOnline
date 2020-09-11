using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations
{
    public partial class Initial6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopEmail",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Shop_Cancelled_Reason",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Shop_Check_In_Date",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Shop_Confirmed_Date",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "User_Check_In_Date",
                table: "AppointmentModel");

            migrationBuilder.AddColumn<DateTime>(
                name: "Appointment_Date",
                table: "AppointmentModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Appointment_Status",
                table: "AppointmentModel",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Barber_Cancelled_Reason",
                table: "AppointmentModel",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Barber_Check_In_Date",
                table: "AppointmentModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Barber_Confirmed_Date",
                table: "AppointmentModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "AppointmentModel",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AppointmentModel",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "User_Booked_Date",
                table: "AppointmentModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Appointment_Date",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Appointment_Status",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Barber_Cancelled_Reason",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Barber_Check_In_Date",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Barber_Confirmed_Date",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "User_Booked_Date",
                table: "AppointmentModel");

            migrationBuilder.AddColumn<string>(
                name: "ShopEmail",
                table: "AppointmentModel",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shop_Cancelled_Reason",
                table: "AppointmentModel",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Shop_Check_In_Date",
                table: "AppointmentModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Shop_Confirmed_Date",
                table: "AppointmentModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AppointmentModel",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "User_Check_In_Date",
                table: "AppointmentModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
