using Domain.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Configurations
{
    public class TokenSettingsValidator : IValidateOptions<TokenSettings>
    {
        public ValidateOptionsResult Validate(string? name, TokenSettings options)
        {
            var failures = new List<string>();

            if (string.IsNullOrEmpty(options.SecretKey))
                failures.Add("TokenSettings.SecretKey is required");
            else if (options.SecretKey.Length < 32)
                failures.Add("TokenSettings.SecretKey must be at least 32 characters long");

            if (string.IsNullOrEmpty(options.Issuer))
                failures.Add("TokenSettings.Issuer is required");

            if (string.IsNullOrEmpty(options.Audience))
                failures.Add("TokenSettings.Audience is required");

            if (options.RefreshTokenExpiryInDays <= 0)
                failures.Add("TokenSettings.RefreshTokenExpiryInDays must be greater than 0");

            if (options.ExpiryInMinutes <= 0)
                failures.Add("TokenSettings.ExpiryInMinutes must be greater than 0");

            if (failures.Any())
                return ValidateOptionsResult.Fail(failures);

            return ValidateOptionsResult.Success;
        }
    }
}
