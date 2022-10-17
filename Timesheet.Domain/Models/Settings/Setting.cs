namespace Timesheet.Domain.Models.Settings
{
    public class Setting : Entity
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Setting(string id) : base(id)
        {
        }
    }
}
