using System;
using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Employees;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;
using Timesheet.Models.Referential;

namespace Timesheet.Application.Timesheets.EventHandlers
{
  internal class TimesheetHandleTimeoffDeleted : BaseEventHandler<TimeoffDeleted>
  {
    private readonly IEmployeeReadRepository _readRepository;
    private readonly IWriteRepository<Employee> _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TimesheetHandleTimeoffDeleted(
        IEmployeeReadRepository readRepository,
        IWriteRepository<Employee> writeRepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
      this._readRepository = readRepository;
      this._writeRepository = writeRepository;
      this._unitOfWork = unitOfWork;
    }

    public override async Task HandleEvent(TimeoffDeleted @event)
    {
      var entry = @event.timeoffDeleted;
      var employee = await _readRepository.GetEmployee(entry.EmployeeId);

      if (employee == null)
      {
        return;
      }

      var currentBenefits = employee.BenefitsVariation ?? new EmployeeBenefits(0, 0, 0);
      var currentSnapshot = employee.BenefitsSnapshot;

      double newVacationHours = currentBenefits.VacationHours;
      double newPersonalHours = currentBenefits.PersonalHours;
      double newRolloverHours = currentBenefits.RolloverHours;

      double newVacationUsed = currentSnapshot.VacationUsed;
      double newPersonalUsed = currentSnapshot.PersonalUsed;

      double newVacationBalance = currentSnapshot.VacationBalance;
      double newPersonalBalance = currentSnapshot.PersonalBalance;

      // Restore hours based on type
      if (entry.TypeId == 5) // Vacation
      {
        newVacationBalance += entry.Hours;
        newVacationUsed -= entry.Hours;
      }
      else if (entry.TypeId == 4) // Personal
      {
        newPersonalBalance += entry.Hours;
        newPersonalUsed -= entry.Hours;
      }

      // Update employee benefits variation
      employee.SetBenefits(newVacationHours, newPersonalHours, newRolloverHours);

      // Update employee benefits snapshot
      var updatedSnapshot = new EmployeeBenefitsSnapshop(currentSnapshot.VacationHours, currentSnapshot.PersonalHours, currentSnapshot.RolloverHours)
        .SetVacationDetails(newVacationBalance, newVacationUsed, currentSnapshot.VacationScheduled)
        .SetPersonalDetails(newPersonalBalance, newPersonalUsed, currentSnapshot.PersonalScheduled);

      employee.SnapshotBenefits(updatedSnapshot);
      employee.UpdateMetadata();

      // Persist changes to database
      _writeRepository.Update(employee);
    }
  }
}
