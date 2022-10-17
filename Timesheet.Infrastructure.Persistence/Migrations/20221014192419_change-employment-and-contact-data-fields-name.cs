using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class changeemploymentandcontactdatafieldsname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmploymentData_TerminationDate",
                table: "Employees",
                newName: "TerminationDate");

            migrationBuilder.RenameColumn(
                name: "EmploymentData_JobTitle",
                table: "Employees",
                newName: "JobTitle");

            migrationBuilder.RenameColumn(
                name: "EmploymentData_IsSalaried",
                table: "Employees",
                newName: "IsSalaried");

            migrationBuilder.RenameColumn(
                name: "EmploymentData_EmploymentDate",
                table: "Employees",
                newName: "EmploymentDate");

            migrationBuilder.RenameColumn(
                name: "EmploymentData_Department",
                table: "Employees",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "Contacts_CompanyPhone",
                table: "Employees",
                newName: "CompanyPhone");

            migrationBuilder.RenameColumn(
                name: "Contacts_CompanyEmail",
                table: "Employees",
                newName: "CompanyEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TerminationDate",
                table: "Employees",
                newName: "EmploymentData_TerminationDate");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "Employees",
                newName: "EmploymentData_JobTitle");

            migrationBuilder.RenameColumn(
                name: "IsSalaried",
                table: "Employees",
                newName: "EmploymentData_IsSalaried");

            migrationBuilder.RenameColumn(
                name: "EmploymentDate",
                table: "Employees",
                newName: "EmploymentData_EmploymentDate");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Employees",
                newName: "EmploymentData_Department");

            migrationBuilder.RenameColumn(
                name: "CompanyPhone",
                table: "Employees",
                newName: "Contacts_CompanyPhone");

            migrationBuilder.RenameColumn(
                name: "CompanyEmail",
                table: "Employees",
                newName: "Contacts_CompanyEmail");
        }
    }
}
