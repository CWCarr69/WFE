using Timesheet.Models.Referential;

namespace Timesheet.Domain.ReadModels.Referential
{


    public class PayrollType
    {
        public int NumId { get; set; }

        public string PayrollCode { get; set; }

        public PayrollTypesCategory Category { get; set; }

        public string ExternalCode { get; set; }
    }
}
