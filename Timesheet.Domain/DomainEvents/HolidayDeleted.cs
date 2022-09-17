using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain.DomainEvents
{
    public record HolidayDeleted(string Id) : IDomainEvent;
}
