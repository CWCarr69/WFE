using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class separatepayrollcodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollCode",
                table: "TimesheetHoliday");

            migrationBuilder.AddColumn<int>(
                name: "PayrollCodeId",
                table: "TimesheetHoliday",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequireApproval",
                table: "PayrollTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollCodeId",
                table: "TimesheetHoliday");

            migrationBuilder.DropColumn(
                name: "RequireApproval",
                table: "PayrollTypes");

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "TimesheetHoliday",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
