using Microsoft.Extensions.Configuration;
using Timesheet.Domain;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.PayrollCodeCorrector
{
    internal class Program
    {
        private static DatabaseService _db;

        private static string _disableConstraints = $@"ALTER TABLE timesheetEntry NOCHECK CONSTRAINT all;
            ALTER TABLE timesheetHoliday NOCHECK CONSTRAINT all;
            ALTER TABLE timesheetException NOCHECK CONSTRAINT all;
            ALTER TABLE notificationitems NOCHECK CONSTRAINT all;
            ALTER TABLE timesheets NOCHECK CONSTRAINT all;";

        private static string _enableConstraints = $@"ALTER TABLE timesheetEntry WITH CHECK CHECK CONSTRAINT all;
            ALTER TABLE timesheetHoliday WITH CHECK CHECK CONSTRAINT all;
            ALTER TABLE timesheetException WITH CHECK CHECK CONSTRAINT all;
            ALTER TABLE notificationitems WITH CHECK CHECK CONSTRAINT all;
            ALTER TABLE timesheets WITH CHECK CHECK CONSTRAINT all;";

        private static string _selectTimesheets = "SELECT * FROM Timesheets";
        private static string _selectTimesheetsEntries = "SELECT * FROM TimesheetEntry";
        private static string _selectTimesheetsHolidays = "SELECT * FROM TimesheetHoliday";

        private static string _removeTimesheetDuplicate = $@"
            DELETE from timesheets where id not in (
            select mid from (
            select startDate, max(id) as mid
            from timesheets
            where payrollPeriod like '%H%'
            group by startDate)t) and id like '%H%';

            DELETE from timesheets where id not in (
            select mid from (
            select startDate, max(id) as mid
            from timesheets
            where payrollPeriod like '%S%'
            group by startDate)t) and id like '%S%';";

        private static string _updateTimesheet = "UPDATE Timesheets set id='{0}',payrollperiod='{0}' Where id='{1}' and startDate='{2}';";
        private static string _updateTimesheetEntry = "UPDATE TimesheetEntry set timesheetHeaderId='{0}' Where timesheetHeaderId='{1}' and id='{2}';";
        private static string _updateTimesheetHoliday = "UPDATE TimesheetHoliday set timesheetHeaderId='{0}' Where timesheetHeaderId='{1}' and id='{2}';";
        private static string _updateTimesheetExceptions = "UPDATE TimesheetException set timesheetHeaderId=(SELECT timesheetheaderId from timesheetEntry WHERE id=TimesheetException.timesheetEntryId);";
        static void Main(string[] args)
        {
            Init();

            Console.WriteLine("DISABLING CONSTRAINTS : " + _disableConstraints);
            _db.Execute(_disableConstraints);

            Console.WriteLine("REMOVING TIMESHEETS DUPLICATES : " + _removeTimesheetDuplicate);
            _db.Execute(_removeTimesheetDuplicate);

            Console.WriteLine("UPDATING TIMESHEETS : " + _selectTimesheets);
            UpdateQueries<Timesheet>(_selectTimesheets, _updateTimesheet, t => t.Id, t => t.StartDate, t => t.StartDate.ToString("yyyy-MM-dd"));
            
            Console.WriteLine("UPDATING TIMESHEETS ENTRIES: " + _selectTimesheetsEntries);
            UpdateQueries<TimesheetEntry>(_selectTimesheetsEntries, _updateTimesheetEntry, t => t.TimesheetHeaderId, t => t.WorkDate, t => t.Id);
            
            Console.WriteLine("UPDATING TIMESHEETS HOLIDAYS: " + _selectTimesheetsHolidays);
            UpdateQueries<TimesheetHoliday>(_selectTimesheetsHolidays, _updateTimesheetHoliday, t => t.TimesheetHeaderId, t => t.WorkDate, t => t.Id);

            Console.WriteLine("UPDATING TIMESHEETS EXCEPTION: " + _updateTimesheetExceptions);
            _db.Execute(_updateTimesheetExceptions);

            Console.WriteLine("ENABLING CONSTRAINTS : " + _enableConstraints);
            _db.Execute(_enableConstraints);
        }

        static void Init()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var connectionString = config.GetConnectionString("Timesheet");
            _db = new DatabaseService(connectionString);
        }

        static void UpdateQueries<T>(string selectQuery, string updateQuery, Func<T, string> timesheetId, Func<T, DateTime> date, Func<T, string> id)
        {
            var items = GetItems<T>(selectQuery);
            /*var updates = */
            items.ToList().ForEach(t =>
            {

                var workDate = date(t);
                var oldId = timesheetId(t);
                var primaryId = id(t);

                var newId = oldId.Contains('H')
                ? TimesheetHeader.CreateWeeklyTimesheet(workDate).Id
                : TimesheetHeader.CreateMonthlyTimesheet(workDate).Id;

                var query = string.Format(updateQuery, newId, oldId, primaryId);
                Console.WriteLine("QUERY ("+workDate+"): " + query);
                _db.Execute(query);
            });
            //.ToList();

            //return string.Join("", updates);
        }

        static List<T> GetItems<T>(string query)
        {
            var items = _db.Query<T>(query);
            return items;
        }
    }
}