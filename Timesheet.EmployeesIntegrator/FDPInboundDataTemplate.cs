﻿namespace Timesheet.FDPDataIntegrator
{
    enum IntegrationType
    {
        EMPLOYEE, PAYROLL
    }

    internal class FDPInboundDataTemplate
    {
        private const string ModificationDateParam = "@ModificationDate";
        private const string PageNumberParam = "@PageNumber";

        private static string _defaultInboudDataTemplate => $@"
            <FPDTS>
                <InboundData>
                    <Records>
                        <Record>
                            <ModifiedAfter>{ModificationDateParam}</ModifiedAfter>
                            <PageNumber>{PageNumberParam}</PageNumber>
                        </Record>
                    </Records>
                </InboundData>
            </FPDTS>
        ";

        private static string _employeeTransfertName = "WilsonFire-Resource-InExport";
        private static string _payrollTransfertName = "WilsonFire-TimesheetDetails-InExport";

        internal static (string transfertName, string inboundData) GetIntegrationParams(IntegrationType type, DateTime? from = null)
        {
            //var fromDate = from ?? DateTime.Now.AddMinutes(-5);
            var fromDate = from ?? DateTime.Now.AddYears(-5);

            var inboundData = _defaultInboudDataTemplate.Replace(ModificationDateParam, fromDate.ToString());
            inboundData = inboundData.Replace(PageNumberParam, "1");

            var transfertName = type == IntegrationType.EMPLOYEE ? _employeeTransfertName : _payrollTransfertName;

            return (transfertName, inboundData);
        }
    }
}
