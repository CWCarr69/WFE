CREATE VIEW TimeoffHours AS
SELECT t.id, t.employeeId, SUM(te.hours) as totalHours
FROM timeoffHeader t
JOIN timeoffEntry te on te.timeoffHeaderId = t.id
GROUP BY t.id, t.employeeId