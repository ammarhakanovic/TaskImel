using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi.Api.Interface;
using UserApi.DataLayer;
using UserApi.DataLayer.Entity;
using UserApi.DataLayer.Models;

namespace UserApi.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHistoryService _historyService;
        public AuditLogController(ApplicationDbContext context, IHistoryService historyService)
        {
            _context = context;
            _historyService = historyService;
        }
        [HttpGet("UserHistory")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetUserHistory(int userId)
        {
            var logs = await _historyService.GetUserHistoryAsync(userId);

            return Ok(logs);
        }

        [HttpGet("log")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogsFiltered([FromQuery] AuditLogFilter filter)
        {
            var logs = await _historyService.GetAuditLogsFilteredAsync(filter);
            return Ok(logs);
        }
    }
}