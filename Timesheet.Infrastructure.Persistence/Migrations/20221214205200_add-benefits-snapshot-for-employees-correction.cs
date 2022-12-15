using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addbenefitssnapshotforemployeescorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BenefitsSnapshot_VacationHours",
                table: "Employees",
                newName: "VacationHours");

            migrationBuilder.RenameColumn(
                name: "BenefitsSnapshot_RolloverHours",
                table: "Employees",
                newName: "RolloverHours");

            migrationBuilder.RenameColumn(
                name: "BenefitsSnapshot_PersonalHours",
                table: "Employees",
                newName: "PersonalHours");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VacationHours",
                table: "Employees",
                newName: "BenefitsSnapshot_VacationHours");

            migrationBuilder.RenameColumn(
                name: "RolloverHours",
                table: "Employees",
                newName: "BenefitsSnapshot_RolloverHours");

            migrationBuilder.RenameColumn(
                name: "PersonalHours",
                table: "Employees",
                newName: "BenefitsSnapshot_PersonalHours");
        }
    }
}
