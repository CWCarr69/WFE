using Timesheet.Domain.Models.Employees;
using Timesheet.Models.Referential;

namespace Timesheet.Domain.Exceptions
{
    public sealed class CannotRequestUnavailableBenefits : Timesheet.Application.ApplicationException
    {
        public CannotRequestUnavailableBenefits(double requestedHours, double availableHours, TimesheetFixedPayrollCodeEnum type)
            : base($"{nameof(TimeoffHeader)}.CannotRequestUnavailableBenefits", 400, $"Requested ({requestedHours}) hours while ({availableHours}) only is available for {type.ToString()}.")
        {
        }
    }
}

