using Duende.IdentityServer;
using IdentityServer;
using IdentityServer.Data;
using IdentityServer.IoC;
using IdentityServer.Pages.Admin.ApiScopes;
using IdentityServer.Pages.Admin.Clients;
using IdentityServer.Pages.Admin.IdentityScopes;
using IdentityServer.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Serilog;

namespace IdentityServer
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            DependancyContainer.RegisterServices(builder.Services);
            DependancyContainer.RegisterDbContext(builder.Services, builder.Configuration);
            DependancyContainer.ConfigureCorsPolicy(builder.Services);
            DependancyContainer.ConfigureIdentityServer(builder.Services, builder.Configuration);

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });


            // this adds the necessary config for the simple admin/config pages
            {
                //builder.Services.AddAuthorization(options =>
                //    options.AddPolicy("admin",
                //        policy => policy.RequireClaim("sub", "1"))
                //);

                //builder.Services.Configure<RazorPagesOptions>(options =>
                //    options.Conventions.AuthorizeFolder("/Admin", "admin"));

                builder.Services.AddTransient<IdentityServer.Pages.Portal.ClientRepository>();
                builder.Services.AddTransient<ClientRepository>();
                builder.Services.AddTransient<IdentityScopeRepository>();
                builder.Services.AddTransient<ApiScopeRepository>();
            }

            // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
            // then enable it
            //isBuilder.AddServerSideSessions();
            //
            // and put some authorization on the admin/management pages using the same policy created above
            //builder.Services.Configure<RazorPagesOptions>(options =>
            //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Content-Security-Policy",
                        "default-src 'self'; " +
                        "script-src 'self' 'unsafe-inline' 'unsafe-eval' http://localhost:3000 https://localhost:3000; " +
                        "style-src 'self' 'unsafe-inline'; " +
                        "connect-src 'self' ws: wss: http://localhost:* https://localhost:*; " +
                        "img-src 'self' data:; " +
                        "font-src 'self' data:;");
                    await next();
                });
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}