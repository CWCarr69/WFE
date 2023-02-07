CREATE VIEW [dbo].TimeoffHeaderRequireApproval 
AS
SELECT the.TimeoffHeaderId, MAX(CAST(pt.RequireApproval as tinyint)) AS RequireApproval
FROM TimeoffEntry the
JOIN PayrollTypes pt on pt.numId = the.TypeId
JOIN TimeoffHeader th on th.Id = the.TimeoffHeaderId
GROUP BY the.TimeoffHeaderId
