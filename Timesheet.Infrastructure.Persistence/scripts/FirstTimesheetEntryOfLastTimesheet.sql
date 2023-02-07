CREATE VIEW FirstTimesheetEntryOfLastTimesheet AS 
SELECT the.TimesheetHeaderId, the.Id AS TimesheetEntryId, th.PayrollPeriod, MIN(the.WorkDate) AS Workdate, the.EmployeeId
FROM TimesheetEntry the
JOIN Timesheets th on th.Id = the.TimesheetHeaderId
GROUP BY the.TimesheetHeaderId, the.Id, th.PayrollPeriod, the.EmployeeId