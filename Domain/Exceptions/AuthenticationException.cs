using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions
{
    public class AuthenticationException : BaseException
    {
        public AuthenticationException(string message)
            : base(message, StatusCodes.Status401Unauthorized, "AUTHENTICATION_FAILED")
        {
        }

    }
}
