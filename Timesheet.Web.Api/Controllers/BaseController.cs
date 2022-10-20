using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Timesheet.Web.Api.Controllers
{
    public abstract class BaseController: ControllerBase
    {
        public string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
