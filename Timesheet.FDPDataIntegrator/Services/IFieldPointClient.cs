using static ServiceReference1.FPDTSWSSoapClient;

namespace Timesheet.FDPDataIntegrator.Services
{
    public interface IFieldPointClient
    {
        public string Response { get; }

        public Task LoadDataAsync(IntegrationType type);
    }
}
