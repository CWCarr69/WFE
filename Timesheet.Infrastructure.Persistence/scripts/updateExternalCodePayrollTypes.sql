alter table payrollTypes alter column externalCode nvarchar(max) null;
update payrollTypes set externalCode = null where numid = '5'
update payrollTypes set externalCode = 'HOL' where numid = '3'
update payrollTypes set externalCode = null where externalCode = ''