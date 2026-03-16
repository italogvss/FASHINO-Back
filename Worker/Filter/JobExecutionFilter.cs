using Hangfire.Server;

namespace Worker.Filter
{
    public class JobExecutionFilter : IServerFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            var jobName = filterContext.BackgroundJob.Job.Type.Name;
            var jobId = filterContext.BackgroundJob.Id;

            Console.WriteLine($"[Hangfire] Starting job: {jobName} (ID: {jobId})");
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            var jobName = filterContext.BackgroundJob.Job.Type.Name;
            var jobId = filterContext.BackgroundJob.Id;
            var duration = filterContext.BackgroundJob.CreatedAt - DateTime.UtcNow;

            if (filterContext.Exception != null)
            {
                Console.WriteLine($"[Hangfire] Job failed: {jobName} (ID: {jobId}) after {duration}s - {filterContext.Exception.Message}");
            }
            else
            {
                Console.WriteLine($"[Hangfire] Job completed: {jobName} (ID: {jobId}) in {duration}s");
            }
        }
    }
}
