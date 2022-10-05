using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addemployeesandtimesheets2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupervisorComment",
                table: "TimeoffHeader",
                newName: "ApproverComment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApproverComment",
                table: "TimeoffHeader",
                newName: "SupervisorComment");
        }
    }
}
