﻿namespace Timesheet.Application.Employees.Commands
{
    public class RejectTimeoff : ICommand
    {
        public string EmployeeId { get; set; }
        public string TimeoffId { get; set; }
        public string Comment { get; set; }
    }
}