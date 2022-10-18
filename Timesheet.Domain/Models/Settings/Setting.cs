namespace Timesheet.Domain.Models.Settings
{
    public class Setting : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Value { get; private set; }

        private Setting(string id, string name, string value, string description) : base(id)
        {
            Name = name;
            Value = value;
            Description = description;
        }

        public static Setting Create(string name, string value, string description)
        {
            var setting = new Setting(GenerateId(), name, value, description);
            return setting;
        }

        public void Update(string name, string value)
        {
            this.Name = name ??  this.Name;
            this.Value = value ??  this.Value;
        }
    }
}
