using Timesheet.Domain.Models.Employees;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator.Employees
{
    internal interface IEmployeeRecordProcessor : IRecordProcessor<IAdapter<EmployeeRecord, Employee>, IRepository<Employee>, EmployeeRecord, Employee>
    { }

    internal class EmployeeRecordProcessor
        : RecordProcessor<IAdapter<EmployeeRecord, Employee>, IRepository<Employee>, EmployeeRecord, Employee>, IEmployeeRecordProcessor
    {
        private readonly IRepository<Employee> _repository;

        public EmployeeRecordProcessor(IRepository<Employee> repository, IAdapter<EmployeeRecord, Employee> adapter)
            : base(repository, adapter)
        {
            this._repository = repository;
        }

        public override async Task Process(EmployeeRecord[] records)
        {
            await _repository.BeginTransaction(async () =>
            {
                _repository.DisableConstraints().Wait();
                base.Process(records).Wait();
                _repository.EnableConstraints().Wait();
            });
        }
    }
}
