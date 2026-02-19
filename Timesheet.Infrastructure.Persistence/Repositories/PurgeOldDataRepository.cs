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

      var dateParam = olderThan.ToString("yyyy-MM-dd");

      try
      {
        // Execute all deletes in a single batch and return all counts
        var purgeAllSql = $@"
          -- Delete TimeoffEntry
          DELETE te
          FROM TimeoffEntry te
          INNER JOIN TimeoffHeader th ON te.TimeoffHeaderId = th.Id
          WHERE th.RequestEndDate < '{dateParam}';
          
          DECLARE @TimeoffEntryCount INT = @@ROWCOUNT;

          -- Delete TimeoffHeader
          DELETE FROM TimeoffHeader 
          WHERE RequestEndDate < '{dateParam}';
          
          DECLARE @TimeoffHeaderCount INT = @@ROWCOUNT;

          -- Delete TimesheetComment
          DELETE tc
          FROM TimesheetComment tc
          INNER JOIN Timesheets t ON tc.TimeSheetId = t.Id
          WHERE t.EndDate < '{dateParam}';
          
          DECLARE @TimesheetCommentCount INT = @@ROWCOUNT;

          -- Delete TimesheetEntry
          DELETE FROM TimesheetEntry 
          WHERE WorkDate < '{dateParam}';
          
          DECLARE @TimesheetEntryCount INT = @@ROWCOUNT;

          -- Delete TimesheetException
          DELETE FROM TimesheetException 
          WHERE CreatedDate < '{dateParam}';
          
          DECLARE @TimesheetExceptionCount INT = @@ROWCOUNT;

          -- Delete Timesheets
          DELETE FROM Timesheets 
          WHERE EndDate < '{dateParam}'

          -- Return all counts in one result set
          SELECT 
            @TimeoffEntryCount AS TimeoffEntry,
            @TimeoffHeaderCount AS TimeoffHeader,
            @TimesheetCommentCount AS TimesheetComment,
            @TimesheetEntryCount AS TimesheetEntry,
            @TimesheetExceptionCount AS TimesheetException;";
        var counts = (await _dbServices.QueryAsync<PurgeCountResult>(purgeAllSql)).FirstOrDefault();
        
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