CREATE VIEW FirstTimesheetEntryOfLastTimesheet AS 
SELECT the.TimesheetHeaderId,
th.status,
th.startDate,
th.endDate, 
th.PayrollPeriod,
the.EmployeeId,
MIN(the.WorkDate) AS Workdate,
MIN(the.status) AS PartialStatus
FROM TimesheetEntry the
JOIN Timesheets th on th.Id = the.TimesheetHeaderId
GROUP BY the.TimesheetHeaderId, the.Id, th.PayrollPeriod, the.EmployeeId, th.status, th.startDate, th.endDate