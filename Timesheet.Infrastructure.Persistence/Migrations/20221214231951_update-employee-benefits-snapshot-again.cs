using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class updateemployeebenefitssnapshotagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_PersonalBalance",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_PersonalScheduled",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_PersonalUsed",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_VacationBalance",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_VacationScheduled",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenefitsSnapshot_VacationUsed",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_PersonalBalance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_PersonalScheduled",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_PersonalUsed",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_VacationBalance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_VacationScheduled",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BenefitsSnapshot_VacationUsed",
                table: "Employees");
        }
    }
}
