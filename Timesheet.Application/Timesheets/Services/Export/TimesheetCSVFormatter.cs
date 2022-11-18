using System.Reflection;
using System.Text;

namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class TimesheetCSVFormatter : ITimesheetCSVFormatter
    {
        private readonly string CSVSeparator = ";";

        private IEnumerable<PropertyInfo> _properties;

        public IEnumerable<PropertyInfo> GetPropertyInfos<T>()
        {
            if(_properties is not null)
            {
                return _properties;
            }
            else
            {
                _properties = typeof(T)
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.GetProperty
                    | BindingFlags.SetProperty);
            }

            return _properties;
        }

        public string Format<T>(TimesheetCSVModel<T> timesheet)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine(AddHeader<T>());

            foreach(var entry in timesheet.Entries)
            {
                csv.AppendLine(AddEntry(entry));
            }

            return csv.ToString();
        }

        private string AddHeader<T>()
        {
            var columns = GetPropertyInfos<T>().Select(a => a.Name).ToArray();
            var header = string.Join(CSVSeparator.ToString(), columns);
            return header;
        }

        private string AddEntry<T>(T entry)
        {
            var csvEntry = new StringBuilder();
            var values = new List<string>();

            foreach (var p in _properties)
            {
                var raw = p.GetValue(entry);
                var value = raw == null 
                    ? string.Empty
                    : raw.ToString().Replace(CSVSeparator, " ");

                values.Add(value);
            }

            csvEntry.Append(string.Join(CSVSeparator, values.ToArray()));
            return csvEntry.ToString();
        }

    }
}
