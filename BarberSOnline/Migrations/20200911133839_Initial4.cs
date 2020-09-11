using Microsoft.EntityFrameworkCore.Migrations;

namespace BarberSOnline.Migrations
{
    public partial class Initial4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Charges",
                table: "AppointmentModel");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "AppointmentModel");

            migrationBuilder.CreateTable(
                name: "AppointmentDetailsModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(nullable: false),
                    Services = table.Column<string>(maxLength: 30, nullable: false),
                    Charges = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDetailsModel", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentDetailsModel");

            migrationBuilder.AddColumn<decimal>(
                name: "Charges",
                table: "AppointmentModel",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "AppointmentModel",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);
        }
    }
}
