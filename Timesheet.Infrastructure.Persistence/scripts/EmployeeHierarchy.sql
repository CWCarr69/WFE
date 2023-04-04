
ALTER VIEW EmployeeHierarchy
AS
WITH EmployeePrimaryManagers AS (
	select e.id as id, e.PrimaryApproverId as managerId, e.id as employeeId
	from employees e 
	where e.Id != '0797'
	UNION ALL
	select e.id, e.PrimaryApproverId, h.employeeId
	FROM employees e 
	JOIN EmployeePrimaryManagers h on e.id = h.managerId
	WHERE e.Id != '0797'
),

EmployeeSecondaryManagers AS (
	select e.id as id, e.SecondaryApproverId as managerId, e.id as employeeId
	from employees e 
	where e.Id != '0797'
	UNION ALL
	select e.id, e.SecondaryApproverId, h.employeeId
	FROM employees e 
	JOIN EmployeeSecondaryManagers h on e.id = h.managerId
	WHERE e.Id != '0797'
),

EmployeeManagers AS (
select employeeId, managerId from EmployeePrimaryManagers 
UNION 
select  employeeId, managerId from EmployeeSecondaryManagers
)

SELECT * FROM EmployeeManagers Where managerId is not null AND len(managerId) > 0
