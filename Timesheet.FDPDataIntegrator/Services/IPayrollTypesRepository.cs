using Timesheet.Domain.ReadModels.Referential;

namespace Timesheet.FDPDataIntegrator.Services
{
    public interface IPayrollTypesRepository
    {
        List<PayrollType> GetPayrollTypes();
    }
}