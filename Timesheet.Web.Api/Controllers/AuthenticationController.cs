using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Infrastructure.Authentication;
using Timesheet.Infrastructure.Authentication.Models;

namespace Timesheet.Web.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationService<AuthenticationResponse> _authenticationService;
        private readonly string _JwtSigningKey ;

        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService<AuthenticationResponse> authenticationService, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            this._authenticationService = authenticationService;
            this._JwtSigningKey = configuration.GetSection("AppSettings:Token").Value;
            this._logger = logger;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Credentials credentials)
        {
            try
            {
                var response = _authenticationService.LogIn(credentials, _JwtSigningKey);
                return Ok(response);
            }catch (Exception ex)
            {
                _logger.LogWarning($"Failed Logon attempt by {credentials.Login}");
                return Unauthorized(ex.Message);
            }
        }
    }
}
