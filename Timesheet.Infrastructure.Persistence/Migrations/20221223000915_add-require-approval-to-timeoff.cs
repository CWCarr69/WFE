using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addrequireapprovaltotimeoff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireApproval",
                table: "TimeoffHeader",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireApproval",
                table: "TimeoffHeader");
        }
    }
}
