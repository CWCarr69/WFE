using Microsoft.Extensions.Logging;
using Timesheet.Application.Employees.Services;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Benefits
{
    public class EmployeeBenefitsService : IEmployeeBenefitsService
    {
        private readonly ILogger<EmployeeBenefitsService> _logger;
        private readonly IEmployeeReadRepository _employeeReadRepository;
        private readonly IEmployeeBenefitCalculator _benefitCalculator;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeBenefitsService(ILogger<EmployeeBenefitsService> logger, 
            IEmployeeReadRepository employeeReadRepository, 
            IEmployeeBenefitCalculator benefitCalculator,
            IUnitOfWork unitOfWork)
        {
            this._logger = logger;
            this._employeeReadRepository = employeeReadRepository;
            this._benefitCalculator = benefitCalculator;
            this._unitOfWork = unitOfWork;
        }

        public async Task UpdateEmployeeBenefits()
        {
            _logger.LogInformation($"[Employee Benefits Services] - Starting global employees Benefits update");

            var allEmployees = _employeeReadRepository.Get().ToList();

            _logger.LogInformation($"[Employee Benefits Services] - Number of rows : {allEmployees.Count()}");
            foreach (var employee in allEmployees)
            {
                await SetEmployeeBenefits(employee);
            }

            _logger.LogInformation($"[Employee Benefits Services] - Global employees Benefits update done");
        }

        private async Task SetEmployeeBenefits(Employee employee)
        {
            if(employee.EmploymentData is null || employee.EmploymentData?.EmploymentDate is null)
            {
                _logger.LogWarning($"[Employee Benefits Services] - No employement Data for employee with Id ({employee.Id})");
                return;
            }

            DateTime employmentDate = (DateTime)employee.EmploymentData?.EmploymentDate;
            var benefits = await _benefitCalculator.GetBenefits(employee.Id, employmentDate);
            var vacationDetails = benefits.Details.FirstOrDefault(x => x.Type == "Vacation");
            var personalDetails = benefits.Details.FirstOrDefault(x => x.Type == "Personal");

            employee.SnapshotBenefits(
                new EmployeeBenefitsSnapshop(benefits.TotalVacationHours, benefits.TotalPersonalHours, benefits.RolloverHours)
                .SetVacationDetails(vacationDetails.Balance, vacationDetails.Used, vacationDetails.Scheduled)
                .SetPersonalDetails(personalDetails.Balance, personalDetails.Used, personalDetails.Scheduled)
            );

            await _unitOfWork.CompleteAsync(CancellationToken.None);
        }
    }
}