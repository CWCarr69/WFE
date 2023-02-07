CREATE VIEW dbo.FirstTimeoffEntryOfLastTimeoff 
AS SELECT the.TimeoffHeaderId, the.Id as TimeoffEntryId, MIN(the.RequestDate) as requestDate
FROM TimeoffEntry the
JOIN TimeoffHeader th on th.Id = the.TimeoffHeaderId
GROUP BY the.TimeoffHeaderId, the.Id