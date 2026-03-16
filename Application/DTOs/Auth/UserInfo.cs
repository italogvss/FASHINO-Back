using Application.DTOs.Subscription;

namespace Application.DTOs.Auth
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Company { get; set; }
        public SubscriptionResponse? Subscription { get; set; }
    }
}
