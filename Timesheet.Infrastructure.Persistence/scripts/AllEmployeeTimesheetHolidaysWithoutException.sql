ALTER VIEW AllEmployeeTimesheetHolidaysWithoutException  
AS SELECT
    tho.Id AS TimesheetEntryId,
    e.Id as EmployeeId,
    e.Fullname as Fullname,
    e.PrimaryApproverId as PrimaryApproverId,
	e.SecondaryApproverId as SecondaryApproverId,
    e.DefaultProfitCenter AS DefaultProfitCenter,
    tho.TimesheetHeaderId AS TimesheetHeaderId,
    tho.WorkDate  as WorkDate,
    pt.numId as PayrollCodeId,
    pt.PayrollCode as PayrollCode,
    tho.Hours as Quantity,
    CAST(null as nvarchar) as CustomerNumber,
    CAST(null as nvarchar) as JobNumber,
    CAST(null as nvarchar) as JobTaskNumber,
    CAST(null as nvarchar) as JobDescription,
    CAST(null as nvarchar) as LaborCode,
    CAST(null as nvarchar) as ServiceOrderNumber,
    e.defaultProfitCenter as ProfitCenterNumber,
    e.Department as Department,
    tho.Description as Description,
    CAST(null as bit) as OutOffCountry,
    tho.Status as TimesheetEntryStatus,
	tho.CreatedDate as CreatedDate,
	tho.ModifiedDate as ModifiedDate,
	0 as IsDeletable,
	1 as isTimeoff,
    1 as isGlobalHoliday
    FROM timesheetHoliday tho
    JOIN timesheets t on t.id = tho.TimesheetHeaderId
    JOIN payrollTypes pt on pt.payrollCode = 'HOLIDAY'
    JOIN employees e on ((e.IsSalaried = 1 AND t.type = 1) OR (e.IsSalaried = 0 AND t.type = 0)) AND e.UsesTimesheet=1
    JOIN AllEmployeeTimesheetPeriods etp on etp.employeeId = e.id AND etp.timesheetHeaderId = tho.TimesheetHeaderId
	LEFT JOIN TimesheetException ho ON ho.TimesheetEntryId = tho.Id And ho.EmployeeId = e.Id
    WHERE ho.Id is null
