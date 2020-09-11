using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations
{
    public partial class Initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentDetailsModel");

            migrationBuilder.DropTable(
                name: "TryAppointment");

            migrationBuilder.AddColumn<decimal>(
                name: "Charges",
                table: "AppointmentModel",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "AppointmentModel",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Charges",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "AppointmentModel");

            migrationBuilder.CreateTable(
                name: "AppointmentDetailsModel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Services = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDetailsModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TryAppointment",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    User_Confirmed_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TryAppointment", x => x.ID);
                });
        }
    }
}
