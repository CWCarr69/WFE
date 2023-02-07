using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class removerequireapprovalfrmtimeoffheader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireApproval",
                table: "TimeoffHeader");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireApproval",
                table: "TimeoffHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
