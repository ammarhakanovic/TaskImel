using System.Threading.Tasks;
using UserApi.DataLayer.Entity;
using UserApi.DataLayer.Models;

namespace UserApi.Api.Interface
{
    public interface IUserService
    {
        Task<PagedResult<User>> GetAllAsync(UserQueryParameters parameters);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(int id, User updatedUser);
        Task<User> DeleteAsync(int id);

    }
}
