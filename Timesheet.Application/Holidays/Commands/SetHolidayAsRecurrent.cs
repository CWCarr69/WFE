using Timesheet.Application.Shared;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.Commands
{
    public class SetHolidayAsRecurrent : ICommand
    {
        public string Id { get; set; }
        public CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}
