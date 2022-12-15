using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Timesheet.Infrastructure.Authentication.Models;

namespace Timesheet.Infrastructure.Authentication.Providers
{
    internal class JwtAuthenticationService : IAuthenticationService<AuthenticationResponse>
    {
        private readonly IAuthenticator _authenticator;
        private readonly double SESSION_LIFE_DAYS = 3;

        public JwtAuthenticationService(IAuthenticator authenticator)
        {
            this._authenticator = authenticator;
        }

        public AuthenticationResponse LogIn(Credentials credentials, string signingKey)
        {
            var user = _authenticator.Authenticate(credentials);
            if (user is null)
            {
                throw new Exception("Invalid credentials. No such user.");
            }

            return GenerateToken(user, signingKey);
        }

        private AuthenticationResponse GenerateToken(User user, string signingKey)
        {
            var claims = new List<Claim>
            {
              new Claim(ClaimTypes.NameIdentifier, user.Id, ClaimValueTypes.String),
              new Claim(ClaimTypes.Name, user.Fullname, ClaimValueTypes.String),
              new Claim(ClaimTypes.Role, user.Role.ToString(), ClaimValueTypes.String)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(SESSION_LIFE_DAYS),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new AuthenticationResponse
            {
                Token = token,
                User = user
            };
        }
    }
}
