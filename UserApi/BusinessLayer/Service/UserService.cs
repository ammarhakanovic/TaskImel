using Microsoft.EntityFrameworkCore;
using UserApi.Api.Interface;
using UserApi.DataLayer;
using UserApi.DataLayer.Entity;
using UserApi.DataLayer.Models;

namespace UserApi.BusinessLayer.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<User>> GetAllAsync(UserQueryParameters parameters)
            {
            try
            {
                var query = _context.Users.AsQueryable();

            if (parameters.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == parameters.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var searchLower = parameters.Search.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }
            var totalCount = await query.CountAsync();

            var skip = (parameters.Page - 1) * parameters.PageSize;
            var items = await query
                .Skip(skip)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new DataLayer.Models.PagedResult<User>
            {
                Items = items,
                TotalCount = totalCount
            };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dohvaćanja svih korisnika: {ex.Message}");
                return new PagedResult<User>();
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dohvaćanja korisnika s ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await HistoryLogActionAsync("Create", user.Id);
                Console.WriteLine($"Korisnik je uspješno kreiran");

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom kreiranja korisnika: {ex.Message}");
                return null!;
            }
        }

        public async Task<User?> UpdateAsync(int id, User updatedUser)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return null;

                var history = new UserLogHistory
                {
                    UserId = existingUser.Id,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    Email = existingUser.Email,
                    IsActive = existingUser.IsActive,
                    ChangedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown",
                    ChangedAt = DateTime.UtcNow
                };
                await _context.HistoryVersions.AddAsync(history);

                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Email = updatedUser.Email;
                existingUser.IsActive = updatedUser.IsActive;

                var auditLog = new AuditLog
                {
                    Action = "Update",
                    EntityId = existingUser.Id,
                    ChangedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown",
                    ChangedAt = DateTime.UtcNow
                };
                await _context.AuditLog.AddAsync(auditLog);

                await _context.SaveChangesAsync();

                Console.WriteLine($"Korisnik s ID {id} uspješno ažuriran");

                return existingUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja korisnika s ID {id}: {ex.Message}");
                return null;
            }
        }


        public async Task<User?> DeleteAsync(int id)
        {
            try
            {
                var deleteUser = await _context.Users.FindAsync(id);
                if (deleteUser == null)
                    return null;

                _context.Users.Remove(deleteUser);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Korisnik s ID {id} uspješno obrisan");
                await HistoryLogActionAsync("Delete", deleteUser.Id);
                return deleteUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja korisnika s ID {id}: {ex.Message}");
                return null;
            }
        }
        private async Task HistoryLogActionAsync(string action, int entityId)
        {
            var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";

            var log = new AuditLog
            {
                Action = action,
                EntityId = entityId,
                ChangedBy = user,
                ChangedAt = DateTime.UtcNow
            };

            await _context.AuditLog.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
