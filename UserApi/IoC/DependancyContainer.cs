using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserApi.Api.Interface;
using UserApi.BusinessLayer.Service;
using UserApi.DataLayer;
namespace UserApi.IoC
{
    public static class DependancyContainer
    {
        public static void RegisterDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("UserDb");
            //var conString = configuration.GetConnectionString("IdentityServerConnection");
            //var migrationsAssembly = typeof(Program).Assembly.GetName().Name;


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

        //    services.AddDbContext<IdentityContext>(options =>
        //        options.UseSqlServer(conString,
        //        sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)));


        //    services.AddIdentity<IdentityUser, IdentityRole>()
        //         .AddEntityFrameworkStores<ApplicationDbContext>()
        //         .AddDefaultTokenProviders();

        }
        public static void ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:5443";
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.Audience = "userApi";
                    options.RequireHttpsMetadata = true;
                });
        }
        public static void ConfigureCors(this IServiceCollection services)
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
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHistoryService, HistoryService>();
        }
    }
}
