select id, payrollperiod from timesheets
select timesheetHeaderId from timesheetEntry
select timesheetHeaderId from timesheetHoliday
select timesheetHeaderId from timesheetException
select objectID from notificationitems where subject like '%timesheet%'

ALTER TABLE timesheetEntry NOCHECK CONSTRAINT all
ALTER TABLE timesheetHoliday NOCHECK CONSTRAINT all
ALTER TABLE timesheetException NOCHECK CONSTRAINT all
ALTER TABLE notificationitems NOCHECK CONSTRAINT all
ALTER TABLE timesheets NOCHECK CONSTRAINT all

update timesheets set id=replace(id, 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)+1)), 
	payrollperiod=replace(id, 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)+1)) 
where payrollperiod like '%H%'

update timesheets set id=replace(id, 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)-2)), 
	payrollperiod=replace(id, 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(id, 6, 6), 'H', ''), 'SS', '') as int)-2)) 
where payrollperiod like '%SS%'

-----------

update timesheetentry set TimesheetHeaderId=replace(TimesheetHeaderId, 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)+1))
where TimesheetHeaderId like '%H%'

update timesheetentry set TimesheetHeaderId=replace(TimesheetHeaderId, 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)-2))
where TimesheetHeaderId like '%SS%'

-----------

update TimesheetHoliday set TimesheetHeaderId=replace(TimesheetHeaderId, 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)+1))
where TimesheetHeaderId like '%H%'

update TimesheetHoliday set TimesheetHeaderId=replace(TimesheetHeaderId, 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)-2))
where TimesheetHeaderId like '%SS%'

-----------

update timesheetException set TimesheetHeaderId=replace(TimesheetHeaderId, 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)+1))
where TimesheetHeaderId like '%H%'

update timesheetException set TimesheetHeaderId=replace(TimesheetHeaderId, 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(TimesheetHeaderId, 6, 6), 'H', ''), 'SS', '') as int)-2))
where TimesheetHeaderId like '%SS%' 

-----------

update notificationitems set objectID=replace(objectID, 
	concat('-', cast(replace(replace(substring(objectID, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(objectID, 6, 6), 'H', ''), 'SS', '') as int)+1))
where objectID like '%H%' and subject like '%timesheet%'

update notificationitems set objectID=replace(objectID, 
	concat('-', cast(replace(replace(substring(objectID, 6, 6), 'H', ''), 'SS', '') as int)), 
	concat('-', cast(replace(replace(substring(objectID, 6, 6), 'H', ''), 'SS', '') as int)-2))
where objectID like '%SS%' and subject like '%timesheet%'


ALTER TABLE timesheetEntry WITH CHECK CHECK CONSTRAINT all
ALTER TABLE timesheetHoliday WITH CHECK CHECK CONSTRAINT all
ALTER TABLE timesheetException WITH CHECK CHECK CONSTRAINT all
ALTER TABLE notificationitems WITH CHECK CHECK CONSTRAINT all
ALTER TABLE timesheets WITH CHECK CHECK CONSTRAINT all


select * from timesheetholiday