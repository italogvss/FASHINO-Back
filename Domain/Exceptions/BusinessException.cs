namespace Domain.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message, 400, "BUSINESS_ERROR") { }
    }
}
