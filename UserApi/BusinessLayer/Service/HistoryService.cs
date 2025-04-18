using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi.Api.Interface;
using UserApi.DataLayer;
using UserApi.DataLayer.Entity;
using UserApi.DataLayer.Models;

namespace UserApi.BusinessLayer.Service
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;
        public HistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<UserLogHistory>> GetUserHistoryAsync(int userId)
        {
            return await _context.HistoryVersions
                                 .Where(h => h.UserId == userId)
                                 .OrderByDescending(h => h.ChangedAt)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<AuditLog>> GetAuditLogsFilteredAsync([FromQuery]AuditLogFilter filter)
        {
            var query = _context.AuditLog.AsQueryable();

            if (!string.IsNullOrEmpty(filter.ChangedBy))
                query = query.Where(log => log.ChangedBy.Contains(filter.ChangedBy));

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(log => log.Action.Equals(filter.Action, StringComparison.OrdinalIgnoreCase));

            if (filter.FromDate.HasValue)
                query = query.Where(log => log.ChangedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(log => log.ChangedAt <= filter.ToDate.Value);

            return await query.OrderByDescending(log => log.ChangedAt).ToListAsync();
        }
    }
}
