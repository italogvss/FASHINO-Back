using Domain.Entities;
using Worker.Models;

namespace Worker.Services.Interfaces
{
    public interface IWorkerConfigService
    {
        Task<List<WorkerConfiguration>> GetAllAsync();
        Task<bool?> Existis(string key);
        Task SeedDefaultsAsync(WorkerConfigs appSettingsConfigs);
    }
}
