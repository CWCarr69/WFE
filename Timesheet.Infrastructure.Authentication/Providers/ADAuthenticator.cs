using System.DirectoryServices.Protocols;
using System.Net;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Settings.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Infrastructure.Authentication.Models;
using User = Timesheet.Infrastructure.Authentication.Models.User;

namespace Timesheet.Infrastructure.Authentication.Providers
{
    internal class ADAuthenticator : IAuthenticator
    {
        private readonly IQuerySetting _querySetting;
        private readonly IQueryEmployee _queryEmployee;

        public ADAuthenticator(IQuerySetting querySetting, IQueryEmployee queryEmployee)
        {
            this._querySetting = querySetting;
            this._queryEmployee = queryEmployee;
        }

        public User Authenticate(Credentials credentials)
        {  
            var adServer = _querySetting.GetSetting("AUTHENTICATION_SERVER").Result;
            if (adServer == null)
            {
                throw new Exception("Active Directory server configuration is missing");
            }
            var connection = new LdapConnection(adServer.Value);
            var credential = new NetworkCredential(credentials.Login, credentials.Password);
            connection.Credential = credential;
            try
            {
                connection.Bind();
            }
            catch (LdapException ex)
            {
               return null;
            }
            finally
            {
                connection.Dispose();
            }

            var employeeId = "0000";
            if(credentials.Login == "UTest1")
            {
                employeeId = "0213";
            }
            else if(credentials.Login == "MTest")
            {
                employeeId = "0645";
            }
            else
            {
                employeeId = "0078";
            }
            //var employee = _queryEmployee.GetEmployeeProfileByLogin(credentials.Login).Result;

            var employee = _queryEmployee.GetEmployeeProfile(employeeId).Result;
            var user = new User
            {
                Id = employee.Id,
                Fullname = employee.FullName,
                Login = employee.Login,
                IsAdministrator = employee.IsAdministrator,
                Role = employee.IsAdministrator ? EmployeeRole.ADMINISTRATOR : EmployeeRole.EMPLOYEE
            };
            return user;
        }
    }
}
