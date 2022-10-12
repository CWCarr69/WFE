using static ServiceReference1.FPDTSWSSoapClient;

namespace Timesheet.FDPDataIntegrator.Services
{
    internal class FieldPointClient
    {
        private readonly FDPSettings _settings;

        public string Response { get; private set; }

        public FieldPointClient(FDPSettings settings)
        {
            this._settings = settings;
        }

        public async Task LoadDataAsync(IntegrationType type)
        {
            try
            {
                var fieldPointClient = new ServiceReference1.FPDTSWSSoapClient(EndpointConfiguration.FPDTSWSSoap12);

                (var transfertName, var inboundData) = FDPInboundDataTemplate.GetIntegrationParams(type);
                Response = await fieldPointClient.TransferAsync(transfertName, inboundData, _settings.FDP_Username, _settings.FDP_Password);

                // Console.Write(Response);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }
}
