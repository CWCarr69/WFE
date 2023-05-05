﻿ALTER VIEW AllEmployeeTimesheetEntriesAndHolidays AS 
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