using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addsettingsnotificationsandothersupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "TimesheetEntry");

            migrationBuilder.DropColumn(
                name: "Benefits_AvailablePersonal",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_AvailableRolloverHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_AvailableVacation",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_ScheduledPersonal",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_ScheduledRolloverHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_ScheduledVacation",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_UsedPersonnal",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_UsedRolloverHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Benefits_UsedVacation",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmploymentData_UnitCost",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmploymentData_YearsInService",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "EmployeeFullName",
                table: "TimesheetEntry",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "PersonalData_CompanyEmail",
                table: "Employees",
                newName: "Contacts_CompanyEmail");

            migrationBuilder.RenameColumn(
                name: "Middlename",
                table: "Employees",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "Lastname",
                table: "Employees",
                newName: "Contacts_CompanyPhone");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Timesheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TimeoffHeader",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TimeoffEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "NotificationItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "NotificationItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Holidays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Initials",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EmploymentData_TerminationDate",
                table: "Employees",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "EmploymentData_IsAdministrator",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_ManagerId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TimeoffHeader");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TimeoffEntry");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "NotificationItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "NotificationItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "EmploymentData_IsAdministrator",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "TimesheetEntry",
                newName: "EmployeeFullName");

            migrationBuilder.RenameColumn(
                name: "Contacts_CompanyEmail",
                table: "Employees",
                newName: "PersonalData_CompanyEmail");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Employees",
                newName: "Middlename");

            migrationBuilder.RenameColumn(
                name: "Contacts_CompanyPhone",
                table: "Employees",
                newName: "Lastname");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Initials",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EmploymentData_TerminationDate",
                table: "Employees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_AvailablePersonal",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_AvailableRolloverHours",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_AvailableVacation",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_ScheduledPersonal",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_ScheduledRolloverHours",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_ScheduledVacation",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_UsedPersonnal",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_UsedRolloverHours",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Benefits_UsedVacation",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EmploymentData_UnitCost",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentData_YearsInService",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
