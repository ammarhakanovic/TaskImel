using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "admin, korisnik", new[] { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(name: "userApi", displayName:"CRUD API")
                {
                    UserClaims = { "role" }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("userApi", "User Api Resource")
                {
                    Scopes = { "userApi" },
                    ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    UserClaims = { "role" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client()
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "userApi" }
                },

                new Client
                {
                    ClientId = "bff",
                    ClientName = "React Frontend",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret  = false,
                    RedirectUris = { "https://localhost:3000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:3000/signout-callback-oidc" },
                    AllowedCorsOrigins = { "https://localhost:3000" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "userApi"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = false,
                    RequirePkce = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 3600
                }
            };
    }
}
