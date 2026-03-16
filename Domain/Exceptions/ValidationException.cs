using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions
{
    public class ValidationException : BaseException
    {
        public IEnumerable<string> ValidationErrors { get; }

        public ValidationException(string message, IEnumerable<string> validationErrors)
            : base(message, StatusCodes.Status400BadRequest, "VALIDATION_ERROR")
        {
            ValidationErrors = validationErrors ?? Array.Empty<string>();
        }
    }
}
