using System.ServiceModel.Description;
using static ServiceReference1.FPDTSWSSoapClient;

namespace Timesheet.EmployeesIntegrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var userId = "YAllabi";
            var username = "FPIntegration@wilsonfire.com";
            var password = "0iRP0qilgyYM7SCipFmp";

            var fieldPointClient = new ServiceReference1.FPDTSWSSoapClient(EndpointConfiguration.FPDTSWSSoap12);
            //await fieldPointClient.ConnectAsync(userId, password);

            var fieldpointTransfertName = "WilsonFire-Resource-InExport";
            //var fieldpointTransfertName = "WilsonFire-TimesheetDetails-InExport";
            var fieldpointInboundData = $@"
                <FPDTS>
                    <InboundData>
                        <Records>
                            <Record>
                                <ModifiedAfter>15 Nov 2019 14:00</ModifiedAfter>
                            </Record>
                        </Records>
                    </InboundData>
                </FPDTS>
            ";
            var responses = await fieldPointClient.TransferAsync(fieldpointTransfertName, fieldpointInboundData, username, password);

            Console.Write(responses);
        }
    }
}