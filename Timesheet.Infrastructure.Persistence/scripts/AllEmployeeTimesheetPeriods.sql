CREATE VIEW [dbo].[AllEmployeeTimesheetPeriods] 
AS
SELECT distinct te.employeeId, te.timesheetHeaderId
FROM timesheetEntry te
LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.Id And tex.EmployeeId = te.EmployeeId
WHERE tex.Id is null 

