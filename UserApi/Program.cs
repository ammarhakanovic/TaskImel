using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserApi.Api.Interface;
using UserApi.BusinessLayer.Service;
using UserApi.DataLayer;
using UserApi.IoC;

var builder = WebApplication.CreateBuilder(args);

DependancyContainer.RegisterServices(builder.Services);
DependancyContainer.RegisterDbContext(builder.Services, builder.Configuration);
//DependancyContainer.ConfigureCors(builder.Services);
DependancyContainer.ConfigureAuthentication(builder.Services);
DependancyContainer.ConfigureCors(builder.Services);


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
