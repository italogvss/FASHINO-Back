using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Defina seus DbSets aqui
        public DbSet<Domain.Entities.Configurations> AppConfig { get; set; } = default!;
        public DbSet<Job> Jobs { get; set; } = default!;
        public DbSet<JobExecution> JobExecution { get; set; } = default!;
        public DbSet<JobMedia> JobMedia { get; set; } = default!;
        public DbSet<Media> Medias { get; set; } = default!;
        public DbSet<Subscription> Subscriptions { get; set; } = default!;
        public DbSet<Plan> Plan { get; set; } = default!;
        public DbSet<SubscriptionEvent> SubscriptionEvents { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<WorkerConfiguration> WorkerConfigurations { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Utilizar string no Enum
            modelBuilder.Entity<Job>()
                .Property(j => j.Type)
                .HasConversion<string>();

            // Configurar todas as propriedades DateTime para usar UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }
        }
    }


}
