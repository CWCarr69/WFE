using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.ReadModels;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    public abstract class BaseController<TController>: ControllerBase
    {
        private readonly ILogger<TController> _logger;

        public BaseController(ILogger<TController> logger)
        {
            this._logger = logger;
        }

        public User CurrentUser => new User
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            IsAdministrator = User.FindFirstValue(ClaimTypes.Role) == EmployeeRole.ADMINISTRATOR.ToString()
        };

        protected PaginatedResult<T> Paginate<T>(int page, int itemsPerPage, WithTotal<T> itemsContainer)
        {
            return new PaginatedResult<T>
            {
                Page = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = itemsContainer?.TotalItems ?? 0,
                Items = itemsContainer?.Items ?? Enumerable.Empty<T>()
            };
        }

        protected PaginatedResult<WithHabilitations<T>> Paginate<T>(int page, int itemsPerPage, int totalItems, List<WithHabilitations<T>> timeoffWithHabilitations)
        {
            return new PaginatedResult<WithHabilitations<T>>
            {
                Page = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = totalItems,
                Items = timeoffWithHabilitations
            };
        }
    
        protected void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
