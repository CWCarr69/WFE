namespace Timesheet.Domain.ReadModels.Employees
{
    public class TimeoffType
    {
        public TimeoffType(Domain.Models.Employees.TimeoffType type)
        {
            Type = type;
            Name = Type.ToString();
        }

        public Domain.Models.Employees.TimeoffType Type { get; set; }
        public string Name { get; set; }

        public static explicit operator TimeoffType(Domain.Models.Employees.TimeoffType domainType)
            => new TimeoffType(domainType);
    }
}
