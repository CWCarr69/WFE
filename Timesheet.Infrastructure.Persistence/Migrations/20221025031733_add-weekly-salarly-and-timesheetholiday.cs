using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addweeklysalarlyandtimesheetholiday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceOrderNumber",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfitCenterNumber",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "JobNumber",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TimesheetHoliday",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayrollCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TimesheetHeaderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetHoliday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimesheetHoliday_Timesheets_TimesheetHeaderId",
                        column: x => x.TimesheetHeaderId,
                        principalTable: "Timesheets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetHoliday_TimesheetHeaderId",
                table: "TimesheetHoliday",
                column: "TimesheetHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimesheetHoliday");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Timesheets");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceOrderNumber",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfitCenterNumber",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobNumber",
                table: "TimesheetEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
