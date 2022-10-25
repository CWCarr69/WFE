using static ServiceReference1.FPDTSWSSoapClient;

namespace Timesheet.FDPDataIntegrator.Services
{
    internal class FieldPointClient : IFieldPointClient
    {
        private readonly FDPSettings _settings;

        public string Response { get; private set; }
        public int MaxRetryTime { get; private set; } = 34;

        public FieldPointClient(ISettingRepository settings)
        {
            this._settings = settings.GetFDPParameters();
        }

        public async Task LoadDataAsync(IntegrationType type)
        {
            long retryTime = 1;
            while (true)
            {

                try
                {
                    var fieldPointClient = new ServiceReference1.FPDTSWSSoapClient(EndpointConfiguration.FPDTSWSSoap12);

                    (var transfertName, var inboundData) = FDPInboundDataTemplate.GetIntegrationParams(type);
                    Response = await fieldPointClient.TransferAsync(transfertName, inboundData, _settings.FDP_Username, _settings.FDP_Password);
                    //File.WriteAllText($@"C:\Users\Hp\Documents\WFE-Timesheet Track\source\Timesheet\Timesheet.EmployeesIntegrator\Services\{type}Sample.xml", Response);
                    //Console.WriteLine(Response);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Retry {retryTime} : {ex}");
                    retryTime = GetNextFibonnaci(retryTime);
                }

                if( retryTime >= MaxRetryTime)
                {
                    return;
                }
            }
        }

        private long GetNextFibonnaci(long number)
        {
            double a = number * (1 + Math.Sqrt(5)) / 2.0;
            return (long) Math.Round(a);
        }
    }
}
