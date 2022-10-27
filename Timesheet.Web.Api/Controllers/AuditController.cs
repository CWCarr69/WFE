using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Audits.Queries;
using Timesheet.Domain.ReadModels.Settings;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IQueryAudit _query;

        public AuditController(IQueryAudit query)
        {
            this._query = query;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditDetails>>> GetAudits(
            string? authorId = null, string? entityId = null, string? type = null, DateTime? from = null)
        {
            var audits = await _query.GetAudits(authorId, entityId, type, from);
            return Ok(audits);
        }
    }
}
