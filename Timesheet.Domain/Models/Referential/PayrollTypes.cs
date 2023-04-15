using Timesheet.Domain;

namespace Timesheet.Models.Referential
{

    public enum TimesheetFixedPayrollCodeEnum
    {
        REGULAR = 1, OVERTIME = 2, HOLIDAY = 3, PERSONAL = 4, VACATION = 5, UNPAID = 6, JURY_DUTY = 7, BERV = 8, SHOP = 9, TRAINING = 10
    }
  
    public enum PayrollTypesCategory
    {
        TIMEOFF=1, BILLABLE = 2
    }

    public class PayrollTypes : Entity
    {
        public static int[] PayrollTypesWithoutApproval =>
            new int[] {
                (int) TimesheetFixedPayrollCodeEnum.SHOP,
                (int) TimesheetFixedPayrollCodeEnum.UNPAID,
                (int) TimesheetFixedPayrollCodeEnum.TRAINING,
            };

        public PayrollTypes(string id) : base(id)
        {
        }

        public PayrollTypes(string id, int numId, string payrollCode, PayrollTypesCategory category, string externalCode, bool requireApproval) : this(id)
        {
            NumId = numId;
            Category = category;
            ExternalCode = externalCode;
            PayrollCode = payrollCode;
            RequireApproval = requireApproval;
        }

        public static PayrollTypes Create(int numId, string payrollCode, PayrollTypesCategory category, string externalCode, bool requireApproval)
        {
            var payrollTypes = new PayrollTypes(GenerateId(), numId, payrollCode, category, externalCode, requireApproval);
            return payrollTypes;
        }

        public int NumId { get; set; }

        public string PayrollCode { get; set; }

        public PayrollTypesCategory Category { get; set; }

        public string ExternalCode { get; set; }
        public bool RequireApproval { get; set; }
    }
}
