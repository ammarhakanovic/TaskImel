﻿using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;
using Duende.IdentityModel;
using System.Security.Claims;

namespace IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                EnsureSeedData(context);
                EnsureUsers(scope);

            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var resource in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");
                foreach (var resource in Config.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiResources already populated");
            }

            if (!context.IdentityProviders.Any())
            {
                Log.Debug("OIDC IdentityProviders being populated");
                context.IdentityProviders.Add(new OidcProvider
                {
                    Scheme = "demoidsrv",
                    DisplayName = "IdentityServer",
                    Authority = "https://demo.duendesoftware.com",
                    ClientId = "login",
                }.ToEntity());
                context.SaveChanges();
            }
            else
            {
                Log.Debug("OIDC IdentityProviders already populated");
            }
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = roleMgr.FindByNameAsync("admin").Result;
            if (adminRole == null)
            {
                adminRole = new IdentityRole("admin");
                var roleResult = roleMgr.CreateAsync(adminRole).Result;
                if (!roleResult.Succeeded)
                {
                    throw new Exception(roleResult.Errors.First().Description);
                }
            }

            var userRole = roleMgr.FindByNameAsync("korisnik").Result;
            if (userRole == null)
            {
                userRole = new IdentityRole("korisnik");
                var roleResult = roleMgr.CreateAsync(userRole).Result;
                if (!roleResult.Succeeded)
                {
                    throw new Exception(roleResult.Errors.First().Description);
                }
            }

            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new IdentityUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim("role", "admin")
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(alice, "Admin").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("alice created and added to Admin role");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new IdentityUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim("location", "somewhere"),
                    new Claim("role", "korisnik")

                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(bob, "Korisnik").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("bob created and added to Korisnik role");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }

    }
}
