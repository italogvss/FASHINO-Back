using Api.Configurations;
using Api.Filter;
using Application.Configurations;
using Domain.Common;
using Infrastructure.Configurations;
using Hangfire;
using Microsoft.Extensions.Options;
using Serilog;

//Program.cs API
var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, services, config) =>
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console());

// DI / Services
builder.Services
    .AddApiHangfire(builder.Configuration)
    .AddFirebase(builder.Configuration)
    .AddTokenService(builder.Configuration)
    .AddStorageService(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiSpecificServices()
    .AddApplicationServices()
    .AddCustomControllers()
    .AddCustomCors()
    .AddCustomSwagger();


//Variaveis de ambiente
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

//Configurações da aplicação
builder.Services.AddOptions<AppConfigurations>()
                    .Bind(builder.Configuration.GetSection("AppConfigurations"))
                    .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<AppConfigurations>, AppConfigurationsValidator>();

var app = builder.Build();

// Middleware
app.UseCustomMiddlewares();

// Swagger e migrations
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
    app.ApplyMigrations();
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new HangfireAuthorizationFilter()] // filtro customizado
    });
}

// Map endpoints
app.MapControllers();

app.Run();
