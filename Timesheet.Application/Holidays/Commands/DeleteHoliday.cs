using Timesheet.Application.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.Commands
{
    public class DeleteHoliday : ICommand
    {
        public string Id { get; set; }
    }
}
