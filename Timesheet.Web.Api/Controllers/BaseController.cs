using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Web.Api.Controllers
{
    public abstract class BaseController: ControllerBase
    {
        public User CurrentUser => new User
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            IsAdministrator = User.FindFirstValue(ClaimTypes.Role) == EmployeeRole.ADMINISTRATOR.ToString()
        };
    }
}
