using Application.Interface.Services;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Services;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configurations
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthAppService, AuthAppService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IJobsService, JobsService>();
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();

            services.AddAutoMapper(cfg =>
            {
                // ✅ Importante para records!
                cfg.ShouldUseConstructor = ci => ci.IsPublic;

                cfg.AddProfile<JobMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<MediaMappingProfile>();
                cfg.AddProfile<JobMappingProfile>();
                //cfg.AddProfile<RevenueCatMappingProfile>());
                cfg.AddProfile<SubscriptionMappingProfile>();
            });

            return services;
        }
    }
}
