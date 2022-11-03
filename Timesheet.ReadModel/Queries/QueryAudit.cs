using Timesheet.Application.Audits.Queries;
using Timesheet.Domain.ReadModels.Settings;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastructure.ReadModel.Queries
{
    public class QueryAudit : IQueryAudit
    {
        private readonly IDatabaseService _dbService;

        public QueryAudit(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public async Task<IEnumerable<AuditDetails>> GetAudits(
            string? authorId = null, string? entityId = null, string? type = null, DateTime? from = null)
        {
            string fromParam = $"from";
            string authorIdParam = $"authorId";
            string entityIdParam = $"entityId";
            string typeParam = $"type";

            var query = $@"SELECT * 
                            FROM audits
                            WHERE 1=1
                            { (from is null ? string.Empty : $"AND date >= {fromParam}") }
                            { (string.IsNullOrEmpty(authorId) ? string.Empty : $"AND authorId = {authorIdParam}") }
                            { (string.IsNullOrEmpty(entityId) ? string.Empty : $"AND entityId = {entityIdParam}") }
                            { (string.IsNullOrEmpty(type) ? string.Empty : $"AND type = {typeParam}") }
                            ORDER BY date DESC";

            var audits = await _dbService.QueryAsync<AuditDetails>(query, new {authorId, entityId, type, from});

            return audits;
        }
    }
}