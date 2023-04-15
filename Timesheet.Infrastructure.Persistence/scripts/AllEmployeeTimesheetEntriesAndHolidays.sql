﻿CREATE VIEW AllEmployeeTimesheetEntriesAndHolidays AS 
select TimesheetEntryId,
    EmployeeId,
    Fullname,
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
	IsTimeoff
from AllEmployeeTimesheetEntriesWithoutException
UNION
select TimesheetEntryId,
    EmployeeId,
    Fullname,
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
	IsTimeoff
from AllEmployeeTimesheetHolidaysWithoutException
