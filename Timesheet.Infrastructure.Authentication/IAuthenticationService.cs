using Timesheet.Infrastructure.Authentication.Models;

namespace Timesheet.Infrastructure.Authentication
{
    public interface IAuthenticationService<TAutorizationResponse>
    {
        TAutorizationResponse LogIn(Credentials credentials, string signingKey=null);
    }
}
