using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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

        [HttpGet("Export")]
        public async Task<IActionResult> ExportTimesheet(
            string? authorId = null, string? entityId = null, string? type = null, DateTime? from = null)
        {
            var audits = await _query.GetAudits(authorId, entityId, type, from);
            var csvData = string.Join(Environment.NewLine, 
                audits.Select(a => $"{a.Entity}; {a.EntityId}; {a.Action}; {a.Type}; {a.Date}; {a.AuthorId}; {a.Data.Replace(";"," ")}")
            );
            var filesBytes = Encoding.UTF8.GetBytes(csvData);

            return File(filesBytes, "text/csv", $"audit_{DateTime.UnixEpoch}.csv");
        }
    }
}
