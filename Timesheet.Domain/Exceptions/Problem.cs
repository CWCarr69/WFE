namespace Timesheet.Domain.Exceptions
{
    public class Problem
    {
        public string Message { get; init; }
        public int Code { get; init; }
        public string Type { get; init; }

        public Problem(string message, int code, string type)
        {
            Message = message;
            Code = code;
            Type = type;
        }
    }
}