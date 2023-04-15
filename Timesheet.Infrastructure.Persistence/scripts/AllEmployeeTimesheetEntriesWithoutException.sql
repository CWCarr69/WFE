CREATE VIEW AllEmployeeTimesheetEntriesWithoutException AS SELECT 
    te.Id AS TimesheetEntryId,
    te.EmployeeId AS EmployeeId,
    e.Fullname as Fullname,
    e.DefaultProfitCenter AS DefaultProfitCenter,
    te.TimesheetHeaderId AS TimesheetHeaderId,
    te.WorkDate  as WorkDate,
    te.PayrollCodeId  as PayrollCodeId,
    pt.PayrollCode  as PayrollCode,
    te.Hours  as Quantity,
    CAST(te.CustomerNumber as nvarchar) as CustomerNumber,
    CAST(te.JobNumber as nvarchar)  as JobNumber,
    te.JobDescription  as JobDescription,
    te.LaborCode  as LaborCode,
    CAST(te.ServiceOrderNumber as nvarchar)  as ServiceOrderNumber,
    COALESCE(te.ProfitCenterNumber, e.DefaultProfitCenter)  AS ProfitCenterNumber,
    e.Department  as Department,
    CASE WHEN pt.Category = 2 THEN te.Description ELSE pt.PayrollCode END as Description,
    CAST(te.OutOffCountry AS bit)  as OutOffCountry,
    te.Status  as TimesheetEntryStatus,
	te.CreatedDate as CreatedDate,
	te.ModifiedDate as ModifiedDate,
	te.IsDeletable as IsDeletable,
	te.IsTimeoff as isTimeoff
    FROM timesheetEntry te
    JOIN employees e on e.Id = te.EmployeeId AND e.UsesTimesheet=1
    JOIN payrollTypes pt on pt.numId = te.PayrollCodeId
    LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.Id And tex.EmployeeId = e.Id
    WHERE tex.Id is null