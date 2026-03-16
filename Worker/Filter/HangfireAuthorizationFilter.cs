using Hangfire.Dashboard;

namespace Worker.Filter
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // TODO: Implementar autenticação apropriada
            // Por enquanto, apenas localhost
            var httpContext = context.GetHttpContext();
            return httpContext.Request.Host.Host == "localhost" ||
                   httpContext.Request.Host.Host == "127.0.0.1";
        }
    }
}
