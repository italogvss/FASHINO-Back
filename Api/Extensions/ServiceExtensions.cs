using Api.Filter;
using Hangfire;
using Hangfire.PostgreSql;
using System.Text.Json;

namespace Api.Configurations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<CustomValidationActionFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                    .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SchemaGeneratorOptions.SchemaIdSelector = type => type.FullName;
                options.UseAllOfToExtendReferenceSchemas();
            });
            return services;
        }

        public static IServiceCollection AddApiSpecificServices(this IServiceCollection services)
        {
            services.AddProblemDetails(opts =>
            {
                // Optional: Configure ProblemDetails options here

            });
            return services;
        }

        public static IServiceCollection AddApiHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("A connection string 'Postgres' não foi encontrada.");
            services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));



            return services;
        }
    }
}
