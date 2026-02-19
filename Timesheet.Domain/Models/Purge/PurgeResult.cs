namespace Timesheet.Domain.Models.Purge
{
  public class PurgeResult
  {
    public DateTime PurgeDate { get; set; }
    public Dictionary<string, int> DeletedCounts { get; set; } = new();
    public int TotalDeleted { get; set; }
    public string Summary => $"Purged {TotalDeleted} total records before {PurgeDate:yyyy-MM-dd}: " +
                             string.Join(", ", DeletedCounts.Select(kvp => $"{kvp.Key}={kvp.Value}"));
  }
}