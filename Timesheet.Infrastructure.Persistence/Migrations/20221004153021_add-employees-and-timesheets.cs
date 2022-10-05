using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Persistence.Migrations
{
    public partial class addemployeesandtimesheets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Holidays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sent = table.Column<bool>(type: "bit", nullable: false),
                    ObjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Population = table.Column<int>(type: "int", nullable: false),
                    Group = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timesheets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayrollPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Middlename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Initials = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PrimaryApproverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SecondaryApproverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmploymentData_JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentData_Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentData_EmploymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentData_TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentData_UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmploymentData_IsSalaried = table.Column<bool>(type: "bit", nullable: false),
                    EmploymentData_YearsInService = table.Column<int>(type: "int", nullable: false),
                    PersonalData_CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Benefits_AvailableVacation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_UsedVacation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_ScheduledVacation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_AvailablePersonal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_UsedPersonnal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_ScheduledPersonal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_AvailableRolloverHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_UsedRolloverHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Benefits_ScheduledRolloverHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_PrimaryApproverId",
                        column: x => x.PrimaryApproverId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Employees_SecondaryApproverId",
                        column: x => x.SecondaryApproverId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Image_PictureId",
                        column: x => x.PictureId,
                        principalTable: "Image",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimesheetEntry",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayrollCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceOrderDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTaskNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaborCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfitCenterNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutOffCountry = table.Column<bool>(type: "bit", nullable: false),
                    TimesheetHeaderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimesheetEntry_Timesheets_TimesheetHeaderId",
                        column: x => x.TimesheetHeaderId,
                        principalTable: "Timesheets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeoffHeader",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupervisorComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeoffHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeoffHeader_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeoffEntry",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TimeoffHeaderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeoffEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeoffEntry_TimeoffHeader_TimeoffHeaderId",
                        column: x => x.TimeoffHeaderId,
                        principalTable: "TimeoffHeader",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PictureId",
                table: "Employees",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PrimaryApproverId",
                table: "Employees",
                column: "PrimaryApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SecondaryApproverId",
                table: "Employees",
                column: "SecondaryApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeoffEntry_TimeoffHeaderId",
                table: "TimeoffEntry",
                column: "TimeoffHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeoffHeader_EmployeeId",
                table: "TimeoffHeader",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetEntry_TimesheetHeaderId",
                table: "TimesheetEntry",
                column: "TimesheetHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "NotificationItems");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "TimeoffEntry");

            migrationBuilder.DropTable(
                name: "TimesheetEntry");

            migrationBuilder.DropTable(
                name: "TimeoffHeader");

            migrationBuilder.DropTable(
                name: "Timesheets");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Holidays");
        }
    }
}
