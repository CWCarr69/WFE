CREATE VIEW AllEmployeeTimesheetPeriods 
AS
SELECT employeeId, max(timesheetHeaderId) AS timesheetHeaderId
FROM timesheetEntry
where status = 3
GROUP BY employeeId