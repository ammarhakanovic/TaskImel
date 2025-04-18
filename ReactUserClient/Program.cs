using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBff()
    .AddRemoteApis();
builder.Services.AddReverseProxy();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
    options.DefaultSignOutScheme = "oidc";
})
.AddCookie("Cookies", options =>
{
    //options.LoginPath = "/Account/Login"; 
    //options.LogoutPath = "/Account/Logout";
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:5443";
    options.ClientId = "bff";
    options.ResponseType = "code";
    options.ResponseMode = "query";

    options.DisableTelemetry = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("userApi");
    options.Scope.Add("offline_access");
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.MapInboundClaims = false;
}); 

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseRouting();
app.UseAuthentication();

app.UseBff();

app.UseAuthorization();
app.MapBffManagementEndpoints();
app.MapRemoteBffApiEndpoint("/bff/user", "https://localhost:6001/api")
.RequireAccessToken(TokenType.User);
app.Run();
