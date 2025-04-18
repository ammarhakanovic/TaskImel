using IdentityServer.Data;
using IdentityServer.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace IdentityServer.IoC
{
    public static class DependancyContainer
    {
        public static void RegisterDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("IdentityServerConnection");
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString,
                   sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = "role";
            })
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();


        }
        public static void ConfigureCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy
                        .WithOrigins("https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });
        }
        public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            services.AddIdentityServer(options =>
               {
                   options.Events.RaiseErrorEvents = true;
                   options.Events.RaiseInformationEvents = true;
                   options.Events.RaiseFailureEvents = true;
                   options.Events.RaiseSuccessEvents = true;

                   // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                   options.EmitStaticAudienceClaim = true;
               })
               .AddDeveloperSigningCredential()
               .AddAspNetIdentity<IdentityUser>()
               // this adds the config data from DB (clients, resources, CORS)
               .AddConfigurationStore(options =>
               {
                   options.ConfigureDbContext = b =>
                       b.UseSqlServer(configuration.GetConnectionString("IdentityServerConnection"),
                   sqlOptions =>
                   {
                       sqlOptions.MigrationsAssembly(migrationsAssembly);
                       sqlOptions.EnableRetryOnFailure();
                   });
               })
               // this is something you will want in production to reduce load on and requests to the DB
               //.AddConfigurationStoreCache()
               //
               // this adds the operational data from DB (codes, tokens, consents)
               .AddOperationalStore(options =>
               {
                   options.ConfigureDbContext = b =>
                       b.UseSqlServer(configuration.GetConnectionString("IdentityServerConnection"),
                   sqlOptions =>
                   {
                       sqlOptions.MigrationsAssembly(migrationsAssembly);
                       sqlOptions.EnableRetryOnFailure();
                   });
               });
        }
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IProfileService, CustomProfileService>();

        }
    }
}
