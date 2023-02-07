using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addtimesheetexceptionandusestimesheetcolumntoemployee2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeId",
                table: "TImesheetException",
                newName: "TimesheetEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimesheetEntryId",
                table: "TImesheetException",
                newName: "TimeId");
        }
    }
}
