CREATE VIEW AllEmployeeTimesheetPeriods 
AS
SELECT employeeId, max(timesheetHeaderId) AS timesheetHeaderId
FROM timesheetEntry
GROUP BY employeeId