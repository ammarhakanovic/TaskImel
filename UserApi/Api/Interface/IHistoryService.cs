using Microsoft.AspNetCore.Mvc;
using UserApi.DataLayer.Entity;
using UserApi.DataLayer.Models;

namespace UserApi.Api.Interface
{
    public interface IHistoryService
    {
        Task<IEnumerable<UserLogHistory>> GetUserHistoryAsync(int userId);
        Task<IEnumerable<AuditLog>> GetAuditLogsFilteredAsync([FromQuery]AuditLogFilter filter);
    }
}
