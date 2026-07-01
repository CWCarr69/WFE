using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain.Models.PayPeriod
{
  public class PayPeriodCreate : Entity
  {
    public string Id { get; set; }
    public DateTime StartDate { get; set; }

    public PayPeriodCreate(string id, DateTime startDate) : base(id)
    {
      StartDate = startDate;
    }
  }
}
