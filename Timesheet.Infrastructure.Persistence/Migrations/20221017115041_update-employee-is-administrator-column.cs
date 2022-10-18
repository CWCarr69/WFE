using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class updateemployeeisadministratorcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmploymentData_IsAdministrator",
                table: "Employees",
                newName: "IsAdministrator");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAdministrator",
                table: "Employees",
                newName: "EmploymentData_IsAdministrator");
        }
    }
}
