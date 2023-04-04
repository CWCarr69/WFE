using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class add_default_profit_center_to_employee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultProfitCenter",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultProfitCenter",
                table: "Employees");
        }
    }
}
