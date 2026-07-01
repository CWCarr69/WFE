using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Models.PayPeriod;
using Timesheet.Domain.Repositories;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
  internal class PayPeriodRepository : IPayPeriodRepository
  {
    private readonly IDatabaseService _dbServices;

    public PayPeriodRepository(IDatabaseService dbServices)
    {
      _dbServices = dbServices;
    }
    public async Task<PayPeriodCreateResult> PayPeriodCreateAsync(DateTime startFrom, CancellationToken cancellationToken)
    {
      var result = new PayPeriodCreateResult
      {
        PayPeriodCreateDate = DateTime.UtcNow,
        CreatedCounts = new Dictionary<string, int>(),
        TotalCreated = 0
      };

      try
      {
        // Your SQL statement to create pay periods based on startFron

        var createSql = @"DECLARE @UpdatedBy NVARCHAR(50) = '-1';
          DECLARE @StartDate DATE = @startFrom;
          BEGIN TRANSACTION;

          -- Find the last period number for Type 0 in the year of @StartDate
          DECLARE @LastHourlyPeriod INT = 0;
          SELECT @LastHourlyPeriod = ISNULL(MAX(CAST(SUBSTRING(PayrollPeriod, CHARINDEX('-', PayrollPeriod) + 1, LEN(PayrollPeriod) - CHARINDEX('-', PayrollPeriod) - 1) AS INT)), 0)
          FROM dbo.Timesheets
          WHERE Type = 0 AND YEAR(StartDate) = YEAR(@StartDate);
          SELECT @LastHourlyPeriod LastHourlyPeriod

          -- Find the last period number for Type 1 in the year of @StartDate
          DECLARE @LastSalariedPeriod INT = 0;
          SELECT @LastSalariedPeriod = ISNULL(MAX(CAST(SUBSTRING(PayrollPeriod, CHARINDEX('-', PayrollPeriod) + 1, LEN(PayrollPeriod) - CHARINDEX('-', PayrollPeriod) - 2) AS INT)), 0)
          FROM dbo.Timesheets
          WHERE Type = 1 AND YEAR(StartDate) = YEAR(@StartDate);
          SELECT @LastSalariedPeriod LastSalariedPeriod

          -- Find next available start date for Type 0, but don't go before @StartDate
          DECLARE @NextHourlyStart DATE;
          SELECT @NextHourlyStart = DATEADD(day, 1, MAX(EndDate))
          FROM dbo.Timesheets
          WHERE Type = 0 AND YEAR(StartDate) = YEAR(@StartDate);
          SELECT @NextHourlyStart NextHourlyStart

          IF @NextHourlyStart IS NULL OR @NextHourlyStart < @StartDate
              SET @NextHourlyStart = @StartDate;

          -- Find next available start date for Type 1, but don't go before @StartDate
          DECLARE @NextSalariedStart DATE;
          SELECT @NextSalariedStart = DATEADD(day, 1, MAX(EndDate))
          FROM dbo.Timesheets
          WHERE Type = 1 AND YEAR(StartDate) = YEAR(@StartDate);
          SELECT @NextSalariedStart NextSalariedStart

          IF @NextSalariedStart IS NULL OR @NextSalariedStart < @StartDate
              SET @NextSalariedStart = @StartDate;

          -- Calculate end date for exactly 12 months from the next start dates
          DECLARE @HourlyEndDate DATE = DATEADD(month, 12, @NextHourlyStart);
          DECLARE @SalariedEndDate DATE = DATEADD(month, 12, @NextSalariedStart);

          -- Type 0: Hourly (weekly pay periods)
          ;WITH WeeklyPeriods AS (
              SELECT 
                  1 AS RowNum,
                  @NextHourlyStart AS StartDate,
                  DATEADD(day, CAST(6 AS BIGINT), @NextHourlyStart) AS EndDate
              UNION ALL
              SELECT 
                  RowNum + 1,
                  DATEADD(day, CAST(7 AS BIGINT), StartDate),
                  DATEADD(day, CAST(7 AS BIGINT), EndDate)
              FROM WeeklyPeriods
              WHERE StartDate < @HourlyEndDate
                AND DATEADD(day, CAST(7 AS BIGINT), StartDate) < @HourlyEndDate
          ),
          NumberedPeriods AS (
              SELECT 
                  StartDate,
                  EndDate,
                  @LastHourlyPeriod + ROW_NUMBER() OVER (ORDER BY StartDate) AS SequentialPeriod,
                  CASE 
                      WHEN (@LastHourlyPeriod + ROW_NUMBER() OVER (ORDER BY StartDate)) > 52
                      THEN ((@LastHourlyPeriod + ROW_NUMBER() OVER (ORDER BY StartDate) - 1) % 52) + 1
                      ELSE @LastHourlyPeriod + ROW_NUMBER() OVER (ORDER BY StartDate)
                  END AS PeriodInYear
              FROM WeeklyPeriods
          ),
          FinalPeriods AS (
              SELECT 
                  CAST(YEAR(StartDate) AS NVARCHAR) + '-' + CAST(PeriodInYear AS NVARCHAR) + 'H' AS Id,
                  CAST(YEAR(StartDate) AS NVARCHAR) + '-' + CAST(PeriodInYear AS NVARCHAR) + 'H' AS PayrollPeriod,
                  StartDate,
                  EndDate
              FROM NumberedPeriods
          )
          INSERT INTO dbo.Timesheets (Id, PayrollPeriod, StartDate, EndDate, Status, CreatedDate, ModifiedDate, UpdatedBy, Type)
          SELECT 
              Id,
              PayrollPeriod,
              StartDate,
              EndDate,
              0,
              GETDATE(),
              GETDATE(),
              @UpdatedBy,
              0
          FROM FinalPeriods
          WHERE NOT EXISTS (
              SELECT 1 
              FROM dbo.Timesheets t
              WHERE t.Id = FinalPeriods.Id
                OR (t.Type = 0 
                    AND t.StartDate = FinalPeriods.StartDate 
                    AND t.EndDate = FinalPeriods.EndDate)
          )
          OPTION (MAXRECURSION 100);

          -- Type 1: Salaried (bi-weekly pay periods)
          ;WITH BiWeeklyPeriods AS (
              SELECT 
                  1 AS RowNum,
                  @NextSalariedStart AS StartDate,
                  DATEADD(day, CAST(13 AS BIGINT), @NextSalariedStart) AS EndDate
              UNION ALL
              SELECT 
                  RowNum + 1,
                  DATEADD(day, CAST(14 AS BIGINT), StartDate),
                  DATEADD(day, CAST(14 AS BIGINT), EndDate)
              FROM BiWeeklyPeriods
              WHERE StartDate < @SalariedEndDate
                AND DATEADD(day, CAST(14 AS BIGINT), StartDate) < @SalariedEndDate
          ),
          NumberedPeriods AS (
              SELECT 
                  StartDate,
                  EndDate,
                  @LastSalariedPeriod + ROW_NUMBER() OVER (ORDER BY StartDate) AS SequentialPeriod,
                  CASE 
                      WHEN (@LastSalariedPeriod + ROW_NUMBER() OVER (ORDER BY StartDate)) > 24
                      THEN ((@LastSalariedPeriod + ROW_NUMBER() OVER (ORDER BY StartDate) - 1) % 24) + 1
                      ELSE @LastSalariedPeriod + ROW_NUMBER() OVER (ORDER BY StartDate)
                  END AS PeriodInYear
              FROM BiWeeklyPeriods
          ),
          FinalPeriods AS (
              SELECT 
                  CAST(YEAR(StartDate) AS NVARCHAR) + '-' + CAST(PeriodInYear AS NVARCHAR) + 'SS' AS Id,
                  CAST(YEAR(StartDate) AS NVARCHAR) + '-' + CAST(PeriodInYear AS NVARCHAR) + 'SS' AS PayrollPeriod,
                  StartDate,
                  EndDate
              FROM NumberedPeriods
          )
          INSERT INTO dbo.Timesheets (Id, PayrollPeriod, StartDate, EndDate, Status, CreatedDate, ModifiedDate, UpdatedBy, Type)
          SELECT 
              Id,
              PayrollPeriod,
              StartDate,
              EndDate,
              0,
              GETDATE(),
              GETDATE(),
              @UpdatedBy,
              1
          FROM FinalPeriods
          WHERE NOT EXISTS (
              SELECT 1 
              FROM dbo.Timesheets t
              WHERE t.Id = FinalPeriods.Id
                OR (t.Type = 1 
                    AND t.StartDate = FinalPeriods.StartDate 
                    AND t.EndDate = FinalPeriods.EndDate)
          )
          OPTION (MAXRECURSION 100);

          SELECT @@ROWCOUNT AS TotalRowsInserted;

          COMMIT TRANSACTION;
          -- ROLLBACK TRANSACTION;";

        // Execute deletes
        await _dbServices.ExecuteAsync(createSql, new { StartFrom = startFrom });
      }
      catch (Exception ex)
      {
        // Log the exception (not implemented here)
        // You might want to set result.CreatedCounts and result.TotalCreated to indicate failure
        throw new Exception("An error occurred while creating pay periods.", ex);
      }
      return result;
    }
  }
}
