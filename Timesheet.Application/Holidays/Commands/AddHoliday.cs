using Timesheet.Application.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.Commands
{
    public class AddHoliday : ICommand
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsRecurrent { get; set; }
    }
}
