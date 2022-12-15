using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addbenefitssnapshotforemployees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VacationHours",
                table: "Employees",
                newName: "VacationHoursSnapshot");

            migrationBuilder.RenameColumn(
                name: "RolloverHours",
                table: "Employees",
                newName: "RolloverHoursSnapshot");

            migrationBuilder.RenameColumn(
                name: "PersonalHours",
                table: "Employees",
                newName: "PersonalHoursSnapshot");

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_PersonalHours",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_RolloverHours",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_VacationHours",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_PersonalHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_RolloverHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_VacationHours",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "VacationHoursSnapshot",
                table: "Employees",
                newName: "VacationHours");

            migrationBuilder.RenameColumn(
                name: "RolloverHoursSnapshot",
                table: "Employees",
                newName: "RolloverHours");

            migrationBuilder.RenameColumn(
                name: "PersonalHoursSnapshot",
                table: "Employees",
                newName: "PersonalHours");
        }
    }
}
