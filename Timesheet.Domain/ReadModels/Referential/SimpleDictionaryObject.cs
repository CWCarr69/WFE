namespace Timesheet.Domain.ReadModels.Referential
{
    public class SimpleDictionaryItem
    {
        private string _value { get; set; }
        public string Key { get; set; }
        public string Value {
            get
            {
                return _value ?? string.Empty;
            }
            set
            {
                _value = value;
            }
        }
    }
}
