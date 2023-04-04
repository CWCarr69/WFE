using Timesheet.Infrastructure.Authentication.Models;

namespace Timesheet.Infrastructure.Authentication
{
    public interface IAuthenticator
    {
        User? Authenticate(Credentials credentials);
    }
}