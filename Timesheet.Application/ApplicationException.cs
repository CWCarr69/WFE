using System;
using Timesheet.Domain.Exceptions;

namespace Timesheet.Application
{
    public class ApplicationException : DomainException
    {
        public ApplicationException(string type, int code, string message) : base(code, message)
        {
        }
    }
}

