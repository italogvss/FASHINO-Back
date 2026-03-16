using Application.DTOs.Common;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Common;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Infrastructure.Configurations
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddFirebase(this IServiceCollection services, IConfiguration configuration)
        {
            var firebaseSettings = configuration.GetSection("Firebase").Get<FirebaseSettings>() ?? throw new InvalidOperationException("A configuration string 'Firebase' não foi encontrada.");
            var firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(firebaseSettings.ServiceAccountKeyPath)
            });
            services.AddSingleton(firebaseApp);
            services.AddSingleton(FirebaseAuth.GetAuth(firebaseApp));
            services.AddScoped<IAuthService, FirebaseAuthService>();

            return services;
        }

        public static IServiceCollection AddStorageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IStorageService, MinIOStorageService>();
            services.AddOptions<MinIOSettings>()
                    .Bind(configuration.GetSection("MinIO"))
                    .ValidateOnStart();
            services.AddSingleton<IValidateOptions<MinIOSettings>, MinIOSettingsValidator>();
            return services;
        }


        public static IServiceCollection AddTokenService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAppTokenService, AppTokenService>();
            services.AddOptions<TokenSettings>()
                    .Bind(configuration.GetSection("TokenSettings"))
                    .ValidateOnStart();
            services.AddSingleton<IValidateOptions<TokenSettings>, TokenSettingsValidator>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Postgres
            var connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("A connection string 'Postgres' não foi encontrada.");
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Infrastructure")));

            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionEventRepository, SubscriptionEventRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobMediaRepository, JobMediaRepository>();
            services.AddScoped<IJobExecutionRepository, JobExecutionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
