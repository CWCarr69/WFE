CREATE VIEW TimesheetHours
AS 
SELECT t.id, te.employeeId, te.fullname, t.PayrollPeriod, t.StartDate, t.EndDate, t.Status, t.CreatedDate, t.ModifiedDate,
min(te.TimesheetEntryStatus) as PartialStatus, 
SUM(te.quantity) as totalHours,
SUM(CASE WHEN te.PayrollCode = 'OVERTIME' THEN te.quantity else 0 END) AS Overtime
FROM timesheets t
JOIN AllEmployeeTimesheetEntriesAndHolidays te on te.timesheetHeaderId = t.id
GROUP BY t.Id, te.employeeId, te.fullname, t.PayrollPeriod, t.StartDate, t.EndDate, t.Status, t.CreatedDate, t.ModifiedDate