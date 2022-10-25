using Timesheet.Domain.ReadModels.Settings;

namespace Timesheet.Application.Audits.Queries
{
    public interface IQueryAudit
    {
        Task<IEnumerable<AuditDetails>> GetAudits(
            string? authorId = null, string? entityId = null, string? type = null, DateTime? from = null);
    }
}
