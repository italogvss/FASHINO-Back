namespace Shared.Interfaces
{
    public interface IJobOrchestrationService
    {
        Task<bool> ShouldRetryAsync(Guid jobExecutionId, string errorCode, CancellationToken cancellationToken = default);
    }
}
