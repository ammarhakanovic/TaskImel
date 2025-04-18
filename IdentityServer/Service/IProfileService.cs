using Duende.IdentityServer.Models;

namespace IdentityServer.Service
{
    public interface IProfileService
    {
        Task GetProfileDataAsync(ProfileDataRequestContext context);
    }
}
