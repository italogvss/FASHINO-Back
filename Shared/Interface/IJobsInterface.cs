using Sahared.Common;

namespace Shared.Interface
{
    public interface IFailedJob
    {
        Task ExecuteAsync(Guid jobExecutionId, FashnRunError errorInfo, CancellationToken cancellationToken = default);
    }

    public interface ISubmitJob
    {
        Task ExecuteAsync(Guid jobExecutionId, CancellationToken cancellationToken = default);
    }

    public interface ICleanUpJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }


    public interface IPollJob
    {
        Task ExecuteAsync(Guid jobExecutionId, int attemptNumber, CancellationToken cancellationToken = default);
    }

    public interface ICompletedJob
    {
        Task ExecuteAsync(Guid jobExecutionId, FashnRunResponse? responseData, CancellationToken cancellationToken = default);
    }
}
