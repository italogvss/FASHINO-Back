using Hangfire.Dashboard;

namespace Api.Filter
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Permite todos (somente dev)
            return true;
        }
    }
}
