using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.Commands
{
    public class UpdateHolidayGeneralInformations : ICommand
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }
}
