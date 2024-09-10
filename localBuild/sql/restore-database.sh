#!/bin/bash

# Wait for SQL Server to start
sleep 30s

# Get logical file names from the backup file
/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P Test12345678* -Q "RESTORE FILELISTONLY FROM DISK = '/var/opt/mssql/backup/back.bak'" -W -s"," > /usr/src/app/filelist.txt

# Extract logical file names
data_file=$(grep ".mdf" /usr/src/app/filelist.txt | awk -F ',' '{print $1}' | tr -d ' ')
log_file=$(grep ".ldf" /usr/src/app/filelist.txt | awk -F ',' '{print $1}' | tr -d ' ')

# Restore the database using the logical file names
/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P Test12345678* -Q "
RESTORE DATABASE Timesheet
FROM DISK = '/var/opt/mssql/backup/back.bak'
WITH FILE = 1,  
MOVE '$data_file' TO '/var/opt/mssql/data/Timesheet.mdf',  
MOVE '$log_file' TO '/var/opt/mssql/data/Timesheet_log.ldf',
NOUNLOAD,  STATS = 5;"
