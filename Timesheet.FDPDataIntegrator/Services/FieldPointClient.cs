using Microsoft.Extensions.Configuration;
using static ServiceReference1.FPDTSWSSoapClient;

namespace Timesheet.FDPDataIntegrator.Services
{
    internal class FieldPointClient : IFieldPointClient
    {
        private readonly FDPSettings _settings;

        public string Response { get; private set; }
        public int MaxRetryTime { get; private set; } = 34;
        private IConfiguration _configuration { get; }

        public FieldPointClient(ISettingRepository settings, IConfiguration configuration)
        {
            this._settings = settings.GetFDPParameters();
            this._configuration = configuration;
        }

        public async Task LoadDataAsync(IntegrationType type, bool all=false)
        {
            long retryTime = 1;
            while (true)
            {

                try
                {
                    var fieldPointClient = new ServiceReference1.FPDTSWSSoapClient(EndpointConfiguration.FPDTSWSSoap12, _settings.FDP_Url);

                    int.TryParse(_configuration.GetSection("FPUpload:MaxHistory").Value, out var historyDays);
                    historyDays = all ? 5 * 365 : historyDays;
                    (var transfertName, var inboundData) = FDPInboundDataTemplate.GetIntegrationParams(type, historyDays == 0 ? 15 : historyDays);
                    
                    Response = await fieldPointClient.TransferAsync(transfertName, inboundData, _settings.FDP_Username, _settings.FDP_Password);
                    
                    if (_settings.FDP_RetainFiles?.ToUpper() == FDPSettings.FDP_RetainFilesTrue)
                    {
                        var basePath = _configuration.GetSection("FPUpload:BaseDir").Value;
                        if(basePath == null)
                        {
                            throw new Exception("No base directory found to persist FP upload files");
                        }
                        File.WriteAllText($@"{basePath}/{type}{DateTime.Now.Ticks}.xml", Response);
                    }
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
