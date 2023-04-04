
using Timesheet.Domain.ReadModels.Referential;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Services;

public class PayrollTypesRepository: IPayrollTypesRepository
{
    private readonly IDatabaseService _dbServices;

    public PayrollTypesRepository(IDatabaseService dbServices)
    {
        _dbServices = dbServices;
    }

    public List<PayrollType> GetPayrollTypes()
    {
        var query = "SELECT * FROM PayrollTypes";
        var payrollTypes = _dbServices.Query<PayrollType>(query);

        return payrollTypes;
    }
}
