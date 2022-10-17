namespace Timesheet.FDPIntegratorService.Service
{
    public interface IFDPIntegratorProcess
    {
        public Task ProcessPayrolls();
        public Task ProcessEmployees();
    }
}
