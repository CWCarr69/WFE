using System.Security.Claims;

namespace Timesheet.Infrastructure.Authentication.Models
{
    public class User
    {
        public string Id { get; internal set; }
        public string Fullname { get; internal set; }
    }
}