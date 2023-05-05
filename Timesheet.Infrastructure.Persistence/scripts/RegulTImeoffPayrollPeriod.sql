update timesheetentry 
set TimesheetHeaderId = replace(TimesheetHeaderId, 'H', 'SS')
from timesheetentry te
Join employees e on te.employeeId = e.id
where te.isTimeoff = 1
and e.isSalaried = 1
and exists (select id from Timesheets where id=replace(TimesheetHeaderId, 'H', 'SS'))

update timesheetentry 
set TimesheetHeaderId = replace(TimesheetHeaderId, 'SS', 'H')
from timesheetentry te
Join employees e on te.employeeId = e.id
where te.isTimeoff = 1
and e.isSalaried = 0
and exists (select id from Timesheets where id=replace(TimesheetHeaderId, 'SS', 'H'))

select distinct *
from (select te.workDate, e.isSalaried, replace(TimesheetHeaderId, 'SS', 'H') AS Period 
from timesheetentry te
Join employees e on te.employeeId = e.id
where te.isTimeoff = 1
and e.isSalaried = 0
and  TimesheetHeaderId like '%SS'
UNION ALL
select te.workDate, e.isSalaried, replace(TimesheetHeaderId, 'H', 'SS') AS Period 
from timesheetentry te
Join employees e on te.employeeId = e.id
where te.isTimeoff = 1
and e.isSalaried = 1
and  TimesheetHeaderId like '%H') t
--where Period not in (select distinct id from timesheets)


