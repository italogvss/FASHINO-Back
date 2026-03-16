using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure
{
    public static class HttpContextExtensions
    {
        public static Guid? GetUserId(this HttpContext httpContext)
        {
            var claim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(claim, out var userId) ? userId : null;
        }
    }
}
