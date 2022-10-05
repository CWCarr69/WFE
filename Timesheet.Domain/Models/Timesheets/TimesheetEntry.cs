﻿using Timesheet.Domain;
namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetEntry : Entity
    {
        public TimesheetEntry(string id) : base(id)
        {
        }

        public string EmployeeId { get; private set; }
        public string EmployeeFullName { get; private set; }
        public DateTime WorkDate { get; private set; }
        public string PayrollCode { get; private set; }
        public double Hours { get; private set; }
        public double Quantity => Hours;
        public string Description { get; private set; }
        public string ServiceOrderNumber { get; private set; }
        public string ServiceOrderDescription { get; private set; }
        public string JobNumber { get; private set; }
        public string JobDescription { get; private set; }
        public string JobTaskNumber { get; private set; }
        public string JobTaskDescription { get; private set; }
        public string LaborCode { get; private set; }
        public string CustomerNumber { get; private set; }
        public string ProfitCenterNumber { get; private set; }
        public string Department { get; private set; }
        public bool OutOffCountry { get; private set; }
        public string WorkArea => OutOffCountry ? "Out of country" : "In state";
    }
}