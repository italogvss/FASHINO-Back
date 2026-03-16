using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions
{
    public class ExternalServiceException : BaseException
    {
        public ExternalServiceException(string message)
            : base(message, StatusCodes.Status502BadGateway, "EXTERNAL_SERVICE_ERROR")
        {
        }

        public ExternalServiceException(string message, Exception innerException)
            : base(message, innerException, StatusCodes.Status502BadGateway, "EXTERNAL_SERVICE_ERROR")
        {
        }
    }
}
