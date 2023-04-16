ALTER VIEW  ExternalEmployeeTimesheetEntries AS
SELECT 
    te.TimesheetHeaderId AS payrollPeriod,
    ('""""""' +  RIGHT('0000' + CAST(te.EmployeeId AS varchar(4)), 4 )+ '""""""') AS No,
    pt.ExternalCode AS PayrollCodeId,
    te.Quantity  as Hours,
    ('""""""'+ cast(te.ProfitCenterNumber as varchar)+'""""""') as CC1,
    IIF(COALESCE(te.JobNumber, te.ServiceOrderNumber) is null or LEN(COALESCE(te.JobNumber, te.ServiceOrderNumber)) = 0, '', ('""""""'+ Left(coalesce(te.JobNumber, te.ServiceOrderNumber),10) +'""""""')) AS Job_Code,
    ('""""""'+Convert(varchar,te.WorkDate,1)+'""""""') AS Begin_Date,
    ('""""""'+Convert(varchar,te.WorkDate,1)+'""""""') AS End_Date
FROM AllEmployeeTimesheetEntriesAndHolidays te
JOIN PayrollTypes pt on pt.numId = te.PayrollCodeId and pt.ExternalCode is not null
WHERE te.timesheetEntryStatus = 3 