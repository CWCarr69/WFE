using Timesheet.Infrastructure.Dapper;
using Microsoft.Extensions.Configuration;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.TimesheetCreator
{
    internal class Period {
        public string EntryId { get; set; }
        public DateTime WorkDate { get; set; }
        public bool IsSalaried { get; set; }
        public TimesheetType Type => IsSalaried ? TimesheetType.SALARLY : TimesheetType.WEEKLY;

        public override bool Equals(object? obj)
        {
            return obj is Period period &&
                   EntryId == period.EntryId &&
                   WorkDate == period.WorkDate &&
                   IsSalaried == period.IsSalaried &&
                   Type == period.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EntryId, WorkDate, IsSalaried, Type);
        }
    }

    internal class Program
    {
        static IDatabaseService _db;
        static Dictionary<string, string> PeriodAndCodes = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            Init();
            List<Period> periods = GetPeriods();
            periods.Distinct().ToList().ForEach(period =>
            {
                CreateTimesheet(period);
            });
            UpdatePeriods(periods);
        }

        private static void Init()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            _db = new DatabaseService(config.GetConnectionString("Timesheet"));
        }

        private static List<Period> GetPeriods()
        {
            var query = $@"
                select distinct *
                from (select te.workDate, e.isSalaried, te.id as EntryId
                from timesheetentry te
                Join employees e on te.employeeId = e.id
                where e.isSalaried = 0
                and  TimesheetHeaderId like '%SS'
                UNION ALL
                select te.workDate, e.isSalaried, te.id as EntryId
                from timesheetentry te
                Join employees e on te.employeeId = e.id
                where e.isSalaried = 1
                and TimesheetHeaderId like '%H') t
            ";
            var periods = _db.Query<Period>(query);

            return periods;
        }

        private static void CreateTimesheet(Period period)
        {

            var timesheet = period.Type == TimesheetType.WEEKLY
                ? TimesheetHeader.CreateWeeklyTimesheet(period.WorkDate)
                : TimesheetHeader.CreateMonthlyTimesheet(period.WorkDate);

            PeriodAndCodes.TryAdd(period.EntryId, timesheet.PayrollPeriod);

            if (TimesheetExists(timesheet.Id))
            {
                return;
            }
            AddTimesheet(timesheet);
        }

        private static bool TimesheetExists(string timesheetId)
        {
            if (_db is null)
            {
                throw new Exception("DatabaseService is not found");
            }

            string timesheetIdParam = "@timesheetId";

            var query = $"Select id from Timesheets where id = {timesheetIdParam}";

            var timesheet = _db.Query<string>(query, new {timesheetId}).FirstOrDefault();

            return timesheet is not null;
        }

        private static void AddTimesheet(TimesheetHeader timesheet)
        {
            if(_db is null)
            {
                throw new Exception("DatabaseService is not found");
            }

            var data = new Dictionary<string, object>
            {
                { nameof(timesheet.Id), timesheet.Id },
                { nameof(timesheet.PayrollPeriod), timesheet.PayrollPeriod },
                { nameof(timesheet.StartDate), timesheet.StartDate },
                { nameof(timesheet.EndDate), timesheet.EndDate },
                { nameof(timesheet.Status), (int)timesheet.Status },
                { nameof(timesheet.CreatedDate), timesheet.CreatedDate },
                { nameof(timesheet.ModifiedDate), timesheet.ModifiedDate },
                { nameof(timesheet.UpdatedBy), timesheet.UpdatedBy },
                { nameof(timesheet.Type), (int)timesheet.Type },
            };

            var query = $@"INSERT INTO 
                Timesheets ({string.Join(",", data.Keys)})
                VALUES ('{string.Join("','", data.Values)}')
            ";

            _db.Execute(query);
        }

        private static void UpdatePeriods(List<Period> periods)
        {
            var query = $@"Update TimesheetEntry 
                Set TimesheetHeaderId = @TimesheetId
                WHERE Id = @Id";

            periods.ForEach(period =>
            {
                _db.Execute(query, new
                {
                    TimesheetId = PeriodAndCodes[period.EntryId],
                    Id = period.EntryId
                });
            });
        }
    }
}