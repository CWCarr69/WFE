USE [Timesheet]
GO

/****** Object: View [dbo].[AllEmployeeTimesheetEntriesAndHolidays] Script Date: 6/6/2023 12:05:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW AllEmployeeTimesheetEntriesAndHolidays AS 
select TimesheetEntryId,
    EmployeeId,
    Fullname,
	PrimaryApproverId,
	SecondaryApproverId,
    DefaultProfitCenter,
    TimesheetHeaderId,
    WorkDate,
    PayrollCodeId,
    PayrollCode,
    Quantity,
    CustomerNumber,
    JobNumber,
    JobTaskNumber,
    JobDescription,
    LaborCode,
    ServiceOrderNumber,
    ProfitCenterNumber,
    Department,
    Description,
    OutOffCountry,
    TimesheetEntryStatus,
	CreatedDate,
	ModifiedDate,
	IsDeletable,
	IsTimeoff,
    isGlobalHoliday
from AllEmployeeTimesheetEntriesWithoutException
UNION
select TimesheetEntryId,
    EmployeeId,
    Fullname,
	PrimaryApproverId,
	SecondaryApproverId,
    DefaultProfitCenter,
    TimesheetHeaderId,
    WorkDate,
    PayrollCodeId,
    PayrollCode,
    Quantity,
    CustomerNumber,
    JobNumber,
    JobTaskNumber,
    JobDescription,
    LaborCode,
    ServiceOrderNumber,
    ProfitCenterNumber,
    Department,
    Description,
    OutOffCountry,
    TimesheetEntryStatus,
	CreatedDate,
	ModifiedDate,
	IsDeletable,
	IsTimeoff,
    isGlobalHoliday
from AllEmployeeTimesheetHolidaysWithoutException
