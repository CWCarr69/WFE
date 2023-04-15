ALTER VIEW dbo.FirstTimeoffEntryOfLastTimeoff 
AS SELECT the.TimeoffHeaderId, th.RequestStartDate, th.RequestEndDate, th.employeeId, th.status
, MIN(the.RequestDate) as requestDate
, MAX(CAST(pt.RequireApproval as tinyint)) AS RequireApproval
FROM TimeoffEntry the
JOIN PayrollTypes pt on pt.numId = the.TypeId
JOIN TimeoffHeader th on th.Id = the.TimeoffHeaderId
GROUP BY the.TimeoffHeaderId, th.RequestStartDate, th.RequestEndDate, th.employeeId, th.status