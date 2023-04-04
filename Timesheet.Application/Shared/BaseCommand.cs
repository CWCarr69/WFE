using System.Text.Json.Serialization;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Shared
{
    public abstract class BaseCommand : ICommand
    {
        [JsonIgnore]
        public User? Author { get; set; }

        public abstract CommandActionType ActionType();
       
    }
}
