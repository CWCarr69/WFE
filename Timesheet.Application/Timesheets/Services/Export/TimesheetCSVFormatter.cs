using System.Reflection;
using System.Text;

namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class TimesheetCSVFormatter : ITimesheetCSVFormatter
    {
        private readonly string CSVSeparator = ";";

        private readonly IEnumerable<PropertyInfo> _properties;

        public TimesheetCSVFormatter()
        {
            _properties = typeof(TimesheetCSVEntryModel)
                .GetProperties(
                    BindingFlags.Public 
                    | BindingFlags.Instance
                    | BindingFlags.GetProperty 
                    | BindingFlags.SetProperty);
        }

        public string Format(TimesheetCSVModel timesheet)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine(AddHeader());

            foreach(var entry in timesheet.Entries)
            {
                csv.AppendLine(AddEntry(entry));
            }

            return csv.ToString();
        }

        private string AddHeader()
        {
            var columns = _properties.Select(a => a.Name).ToArray();
            var header = string.Join(CSVSeparator.ToString(), columns);
            return header;
        }

        private string AddEntry(TimesheetCSVEntryModel entry)
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
