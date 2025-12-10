using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Holidays;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;
using Timesheet.Models.Referential;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
  internal sealed class TimesheetHandleHolidayAdded : BaseEventHandler<HolidayAdded>
  {
    private readonly ITimesheetReadRepository _readRepository;
    private readonly IWriteRepository<TimesheetHeader> _writeRepository;
    private readonly IEmployeeReadRepository _employeeRepository;


    public TimesheetHandleHolidayAdded(
            ITimesheetReadRepository readRepository,
            IWriteRepository<TimesheetHeader> writeRepository,
            IEmployeeReadRepository employeeRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
    {
      this._readRepository = readRepository;
      this._writeRepository = writeRepository;
      this._employeeRepository = employeeRepository;
    }

    public override async Task HandleEvent(HolidayAdded @event)
    {
      var timesheets = (await _readRepository.GetTimesheetByDate(@event.Date))
          ?.ToList() ?? new List<TimesheetHeader>();

      if (!timesheets.Any(t => t.Type == TimesheetType.WEEKLY))
      {
        var timesheetWeekly = TimesheetHeader.CreateWeeklyTimesheet(@event.Date);
        await _writeRepository.Add(timesheetWeekly);
        timesheets.Add(timesheetWeekly);
      }

      if (!timesheets.Any(t => t.Type == TimesheetType.SALARLY))
      {
        var timesheetMonthly = TimesheetHeader.CreateMonthlyTimesheet(@event.Date);
        await _writeRepository.Add(timesheetMonthly);
        timesheets.Add(timesheetMonthly);
      }
      // employees eligible for holiday entry must have been hired at least 90 days before holiday
      var hireThreshold = @event.Date.AddDays(-90).Date;
      var employees = _employeeRepository.Get()
          .Where(e => e.IsActive
                      && e.UsesTimesheet
                      && e.EmploymentData is not null
                      && e.EmploymentData.EmploymentDate <= hireThreshold)
          .ToList();

      foreach (var timesheet in timesheets)
      {
        var existingTimesheet = await _readRepository.Get(timesheet.Id);
        existingTimesheet.AddHoliday(TimesheetHoliday.Create(@event.Id, @event.Date, @event.Description));

        // add timesheet entry for holiday for eligible employees if one does not already exist
        foreach (var employee in employees)
        {
          var exists = existingTimesheet.TimesheetEntries
              .Any(te => te.EmployeeId == employee.Id
                         && te.WorkDate.Date == @event.Date.Date
                         && te.PayrollCodeId == (int)TimesheetFixedPayrollCodeEnum.HOLIDAY);

          if (!exists)
          {
            var entryId = Guid.NewGuid().ToString();
            var entry = new TimesheetEntry(
                entryId,
                employee.Id,
                @event.Date.Date,
                (int)TimesheetFixedPayrollCodeEnum.HOLIDAY,
                8, // default holiday hours
                @event.Description ?? string.Empty,
                null, // serviceOrderNumber
                null, // serviceOrderDescription
                null, // jobNumber
                null, // jobDescription
                null, // jobTaskNumber
                null, // profitCenter
                false // outOffCountry
            );

            existingTimesheet.AddTimesheetEntry(entry);
          }
        }

      }
    }
  }
}
