using Shared.Interface;
using Worker.Filter;
using Worker.Jobs;
using Hangfire;
using Hangfire.PostgreSql;

namespace Worker.Configuration
{
    public static class HangfireConfiguration
    {
        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration configuration,
            int workerCount = 10)
        {
            var connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("A connection string 'Postgres' não foi encontrada.");
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UsePostgreSqlStorage(connectionString)
                      .UseFilter(new JobExecutionFilter())
                      .UseFilter(new AutomaticRetryAttribute
                      {
                          Attempts = 3,
                          DelaysInSeconds = new[] { 60, 300, 900 } // 1min, 5min, 15min
                      });
            });

            // Configura Hangfire Server
            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "critical", "default", "low" };
                options.WorkerCount = workerCount;
                options.ServerName = $"FashnWorker-{Environment.MachineName}";
                options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
                options.HeartbeatInterval = TimeSpan.FromSeconds(30);
                options.ServerTimeout = TimeSpan.FromMinutes(5);
                options.ServerCheckInterval = TimeSpan.FromMinutes(1);
            });

            return services;
        }

        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            // Registra os jobs
            services.AddScoped<ISubmitJob, SubmitJobToFashnJob>();
            services.AddScoped<IPollJob, PollJobStatusJob>();
            services.AddScoped<ICompletedJob, ProcessCompletedJobJob>();
            services.AddScoped<IFailedJob, ProcessFailedJobJob>();
            services.AddScoped<ICleanUpJob, CleanupExpiredJobsJob>();
            return services;
        }
        public static void ConfigureRecurringJobs(IServiceProvider serviceProvider)
        {
            var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

            // Job de limpeza - executa diariamente às 3h da manhã
            recurringJobManager.AddOrUpdate<CleanupExpiredJobsJob>(
                recurringJobId: "cleanup-expired-jobs",
                methodCall: job => job.ExecuteAsync(CancellationToken.None),
                cronExpression: Cron.Daily(3),
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = "low"
                });
        }
    }
}
