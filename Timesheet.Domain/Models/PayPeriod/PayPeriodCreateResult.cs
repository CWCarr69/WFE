using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain.Models.PayPeriod
{
  public class PayPeriodCreateResult
  {
    public DateTime PayPeriodCreateDate { get; set; }
    public Dictionary<string, int> CreatedCounts { get; set; } = new();
    public int TotalCreated { get; set; }
    public string Summary => $"Pay Periods created {TotalCreated} total records before {PayPeriodCreateDate:yyyy-MM-dd}: " +
                             string.Join(", ", CreatedCounts.Select(kvp => $"{kvp.Key}={kvp.Value}"));
  }
}
