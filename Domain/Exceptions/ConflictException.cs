using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base(message, StatusCodes.Status409Conflict, "RESOURCE_CONFLICT")
        {
        }

    }
}
