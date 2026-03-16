using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        [MaxLength(100)]
        public string FirebaseUid { get; private set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; private set; }

        [MaxLength(100)]
        public string? DisplayName { get; private set; }

        [MaxLength(100)]
        public string? Company { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        public DateTime LastLoginAt { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public string? RefreshToken { get; private set; }

        public DateTime? RefreshTokenExpiresAt { get; private set; }

        public DateTime? RefreshTokenCreatedAt { get; private set; }

        public Guid? SubscriptionId { get; private set; }

        [ForeignKey(nameof(SubscriptionId))]
        public virtual Subscription? Subscription { get; private set; }

        // Relação 1:N -> User tem muitas Jobs
        public virtual ICollection<Job> Jobs { get; private set; } = new List<Job>();

        public User() { }

        public User(string firebaseUid, string email, string? displayName = null)
        {
            Id = Guid.NewGuid();
            FirebaseUid = string.IsNullOrWhiteSpace(firebaseUid) ? throw new ArgumentNullException(nameof(firebaseUid)) : firebaseUid;
            Email = email;
            DisplayName = displayName;
            CreatedAt = DateTime.UtcNow;
            LastLoginAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
        public bool IsActiveUser() => IsActive;
        public void UpdateLoginTime() => LastLoginAt = DateTime.UtcNow;

        public void UpdateProfile(string? email, string? displayName)
        {
            if (!string.IsNullOrWhiteSpace(email)) Email = email;
            if (!string.IsNullOrWhiteSpace(displayName)) DisplayName = displayName;

            LastLoginAt = DateTime.UtcNow;
        }

        public void UpdateRefreshToken(string refreshToken, DateTime expiresAt)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiresAt = expiresAt;
            RefreshTokenCreatedAt = DateTime.UtcNow;
        }

        public bool IsRefreshTokenValid(string refreshToken)
        {
            return !string.IsNullOrEmpty(RefreshToken) &&
                   RefreshToken == refreshToken &&
                   RefreshTokenExpiresAt.HasValue &&
                   RefreshTokenExpiresAt.Value > DateTime.UtcNow;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiresAt = null;
            RefreshTokenCreatedAt = null;
        }

        public void UpdateSubscription(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
            IsActive = true;
        }

        public void RemoveSubscription()
        {
            SubscriptionId = null;
        }
    }
}

