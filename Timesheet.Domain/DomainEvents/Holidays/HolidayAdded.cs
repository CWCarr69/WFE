using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain.DomainEvents
{
    public record HolidayAdded(DateTime date, string description) : IDomainEvent;
}
