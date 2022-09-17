
namespace Timesheet.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public int Code { get; }
        public string Type { get; }

        public DomainException(string message) : base(message) 
        {
        }

        public DomainException(int code, string message) : this(message)
        {
            Code = code;
        }

        public DomainException(string type, int code, string message) : this(code, message)
        {
            Code = code;
            Type = type;
        }

        public Problem ToProblem()
        {
            return new Problem(Message, Code, Type);
        }
    }
}
