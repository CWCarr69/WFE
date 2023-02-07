using System.Xml.Serialization;

namespace Timesheet.FDPDataIntegrator.Employees
{
    [XmlRoot("OutboundData")]
    public class EmployeeRecords
    {
        [XmlArray("Records")]
        [XmlArrayItem("Record")]
        public EmployeeRecord[] Records { get; set; }
    }

    public class EmployeeRecord
    {
        public string EmployeeCode { get; set; }
        [XmlElement("AzureUserID")]
        public string ADLogin { get; set; }
        [XmlElement("Name")]
        public string FullName { get; set; }
        public string Title { get; set; }
        [XmlElement("ManagerEmployeeCode")]
        public string ManagerId { get; set; }
        [XmlElement("JobRole")]
        public string JobRole { get; set; }
        [XmlElement("EmployeeCategory")]
        public string Category { get; set; }
        public string Department { get; set; }
        [XmlElement("EmploymentDate")]
        public DateTime EmploymentDate { get; set; }
        public string Email { get; set; }
        [XmlElement("Phone1")]
        public string Phone { get; set; }
        public string Active { get; set; }
        [XmlElement("PayrollType")]
        public string TimesheetPeriod { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Status { get; set; }
        [XmlElement("UDF_SecondaryApprover")]
        public string SecondaryApproverId { get; set; }
        [XmlElement("UsesTimeSheets")]
        public string UsesTimesheet { get; set; } 

    }
}
