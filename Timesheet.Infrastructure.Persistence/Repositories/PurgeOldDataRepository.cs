using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Models.Purge;
using Timesheet.Domain.Repositories;
using Timesheet.Infrastructure.Dapper;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
  internal class PurgeOldDataRepository : IPurgeOldDataRepository
  {
    private readonly IDatabaseService _dbServices;

    public PurgeOldDataRepository(IDatabaseService dbService)
    {
      _dbServices = dbService;
    }

    public async void PurgeOldDataAsync(DateTime olderThan, CancellationToken token)
    {
      // Example: Purge from Timesheets table;
      var sql = $@"DELETE FROM TimeoffEntry WHERE TimeoffHeaderId in (SELECT Id FROM TimeoffHeader WHERE RequestEndDate < '{olderThan}');
          DELETE FROM TimeoffHeader WHERE RequestEndDate < '{olderThan}';
          DELETE FROM TimesheetComment WHERE TimeSheetId in (SELECT Id FROM Timesheets WHERE EndDate <'{olderThan}');
          DELETE FROM TimesheetEntry WHERE WorkDate <'{olderThan}';
          DELETE FROM TimesheetException WHERE CreatedDate <'{olderThan}';
          DELETE FROM Timesheets WHERE EndDate <'{olderThan}';";
      await _dbServices.ExecuteAsync(sql);
      
    }

  }
}