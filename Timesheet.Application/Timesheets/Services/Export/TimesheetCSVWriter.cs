using System.Text;

namespace Timesheet.Application.Timesheets.Services.Export
{
    public class TimesheetCSVWriter : ITimesheetCSVWriter
    {
        private string _path;

        public void SetPath(string path)
        {
            _path = path;
        }

        public async Task Write(string csv)
        {
            if(csv is null)
            {
                return;
            }

            byte[] encodedText = Encoding.Unicode.GetBytes(csv);

            using (FileStream sourceStream = new FileStream(_path,
                FileMode.Truncate, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
    }
}
