using Timesheet.Domain.Models.Holidays;

namespace Timesheet.Domain.Exceptions
{
    public sealed class HolidayAlreadyExistException : DomainException
    {
        public HolidayAlreadyExistException(DateTime date) 
        : base("Holiday.AlreadyExist", 400, $"{nameof(Holiday)} already exist on {date}.")
        {
        }
    }
}
