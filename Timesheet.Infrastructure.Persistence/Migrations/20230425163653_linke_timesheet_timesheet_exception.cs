using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class linke_timesheet_timesheet_exception : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimesheetHeaderId",
                table: "TImesheetException",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TImesheetException_TimesheetHeaderId",
                table: "TImesheetException",
                column: "TimesheetHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_TImesheetException_Timesheets_TimesheetHeaderId",
                table: "TImesheetException",
                column: "TimesheetHeaderId",
                principalTable: "Timesheets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TImesheetException_Timesheets_TimesheetHeaderId",
                table: "TImesheetException");

            migrationBuilder.DropIndex(
                name: "IX_TImesheetException_TimesheetHeaderId",
                table: "TImesheetException");

            migrationBuilder.DropColumn(
                name: "TimesheetHeaderId",
                table: "TImesheetException");
        }
    }
}
