namespace Timesheet.Domain.ReadModels.Settings
{
    public class SettingDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
    }
    public class SettingDetailsGrouped
    {
        public string Group { get; set; }
        public IEnumerable<SettingDetails> Settings { get; set; }
    }
}
