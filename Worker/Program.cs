using Application.Configurations;
using Application.Interfaces.Services;
using Domain.Common;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Shared.Interfaces;
using Worker.Configuration;
using Worker.Services;
using Worker.Services.Interface;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);


//DI
builder.Services.AddInfrastructureServices(builder.Configuration)
                .AddStorageService(builder.Configuration)
                .AddApplicationServices()
                .AddHangfireServices(builder.Configuration)
                .AddJobs();

// Services
builder.Services.AddHttpClient<IHttpClient, HttpClientService>();
builder.Services.AddScoped<IFashnAIService, FashnAiService>();
builder.Services.AddScoped<IJobOrchestrationService, JobOrchestrationService>();
builder.Services.AddScoped<IStorageService, MinIOStorageService>();

//Configurações
builder.Services.AddOptions<AppConfigurations>()
        .Bind(builder.Configuration.GetSection("AppConfigurations"))
        .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<AppConfigurations>, AppConfigurationsValidator>();
builder.Services.AddOptions<FashnAiConfiguration>()
        .Bind(builder.Configuration.GetSection("FashnAiConfiguration"))
        .ValidateOnStart();

var host = builder.Build();

// RUN HOST
await host.RunAsync();
