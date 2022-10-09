using System.Xml.Serialization;

namespace Timesheet.EmployeesIntegrator.Payrolls
{
    [XmlRoot("Record")]
    internal class PayrollRecord
    {
        public string EmployeeCode { get; set; }
        [XmlElement("PayCode")]
        public string PayrollCode { get; set; }
        public DateTime WorkDate { get; set; }
        public string ProfitCenter { get; set; }
        public double Quantity { get; set; }
        [XmlElement("ProjectNo")]
        public string ServiceOrderNumber { get; set; }
        public string JobNumber { get; set; }
        [XmlElement("record")]
        public string RecordId { get; set; }
        public string CustomerName { get; set; }
    }
}
