using Timesheet.Application.Shared;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.Commands
{
    public class UpdateHolidayGeneralInformations : BaseCommand
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}
