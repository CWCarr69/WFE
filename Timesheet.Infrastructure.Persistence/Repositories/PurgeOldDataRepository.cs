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

    public async Task<PurgeResult> PurgeOldDataAsync(DateTime olderThan, CancellationToken token)
    {
      var result = new PurgeResult
      {
        PurgeDate = olderThan,
        DeletedCounts = new Dictionary<string, int>()
      };

      try
      {
        // First execute all deletes
        var deleteSql = @"
          -- Delete TimeoffEntry
          DELETE te
          FROM TimeoffEntry te
          INNER JOIN TimeoffHeader th ON te.TimeoffHeaderId = th.Id
          WHERE th.RequestEndDate < @OlderThan;

          -- Delete TimeoffHeader
          DELETE FROM TimeoffHeader 
          WHERE RequestEndDate < @OlderThan;

          -- Delete TimesheetComment
          DELETE tc
          FROM TimesheetComment tc
          INNER JOIN Timesheets t ON tc.TimeSheetId = t.Id
          WHERE t.EndDate < @OlderThan;

          -- Delete TimesheetEntry
          DELETE FROM TimesheetEntry 
          WHERE WorkDate < @OlderThan;

          -- Delete TimesheetException
          DELETE FROM TimesheetException 
          WHERE CreatedDate < @OlderThan;

          -- Delete Timesheets
          DELETE FROM Timesheets 
          WHERE EndDate < @OlderThan;";

        // Then get counts separately
        var countSql = @"
          SELECT 
            (SELECT COUNT(*) FROM TimeoffEntry te 
             INNER JOIN TimeoffHeader th ON te.TimeoffHeaderId = th.Id
             WHERE th.RequestEndDate < @OlderThan) AS TimeoffEntry,
            (SELECT COUNT(*) FROM TimeoffHeader WHERE RequestEndDate < @OlderThan) AS TimeoffHeader,
            (SELECT COUNT(*) FROM TimesheetComment tc 
             INNER JOIN Timesheets t ON tc.TimeSheetId = t.Id
             WHERE t.EndDate < @OlderThan) AS TimesheetComment,
            (SELECT COUNT(*) FROM TimesheetEntry WHERE WorkDate < @OlderThan) AS TimesheetEntry,
            (SELECT COUNT(*) FROM TimesheetException WHERE CreatedDate < @OlderThan) AS TimesheetException,
            (SELECT COUNT(*) FROM Timesheets WHERE EndDate < @OlderThan) AS Timesheets;";

        var counts = (await _dbServices.QueryAsync<PurgeCountResult>(countSql, new { OlderThan = olderThan })).FirstOrDefault();

        // Execute deletes
        await _dbServices.ExecuteAsync(deleteSql, new { OlderThan = olderThan });

        if (counts != null)
        {
          result.DeletedCounts["TimeoffEntry"] = counts.TimeoffEntry;
          result.DeletedCounts["TimeoffHeader"] = counts.TimeoffHeader;
          result.DeletedCounts["TimesheetComment"] = counts.TimesheetComment;
          result.DeletedCounts["TimesheetEntry"] = counts.TimesheetEntry;
          result.DeletedCounts["TimesheetException"] = counts.TimesheetException;
          result.DeletedCounts["Timesheets"] = counts.Timesheets;
          result.TotalDeleted = result.DeletedCounts.Values.Sum();
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Purge failed: {ex.Message}. Inner: {ex.InnerException?.Message}", ex);
      }

      return result;
    }

    // Helper class to map the result
    private class PurgeCountResult
    {
      public int TimeoffEntry { get; set; }
      public int TimeoffHeader { get; set; }
      public int TimesheetComment { get; set; }
      public int TimesheetEntry { get; set; }
      public int TimesheetException { get; set; }
      public int Timesheets { get; set; }
    }
  }
}