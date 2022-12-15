using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class reviewpayrolltypesmodelsandemployeebenefits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollCode",
                table: "TimesheetEntry");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "TimeoffEntry",
                newName: "TypeId");

            migrationBuilder.AddColumn<int>(
                name: "PayrollCodeId",
                table: "TimesheetEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ConsiderFixedBenefits",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "PersonalHours",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RolloverHours",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VacationHours",
                table: "Employees",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "PayrollTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumId = table.Column<int>(type: "int", nullable: false),
                    PayrollCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTypes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayrollTypes");

            migrationBuilder.DropColumn(
                name: "PayrollCodeId",
                table: "TimesheetEntry");

            migrationBuilder.DropColumn(
                name: "ConsiderFixedBenefits",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PersonalHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RolloverHours",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "VacationHours",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "TimeoffEntry",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
