using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound, "RESOURCE_NOT_FOUND")
        {
        }
    }
}
