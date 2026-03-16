using Domain.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Configurations
{
    public class MinIOSettingsValidator : IValidateOptions<MinIOSettings>
    {
        public ValidateOptionsResult Validate(string? name, MinIOSettings options)
        {
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Endpoint))
                failures.Add("MinIOSettings.Endpoint is required");

            if (string.IsNullOrWhiteSpace(options.AccessKey))
                failures.Add("MinIOSettings.AccessKey is required");

            if (string.IsNullOrWhiteSpace(options.SecretKey))
                failures.Add("MinIOSettings.SecretKey is required");

            if (string.IsNullOrWhiteSpace(options.BucketName))
                failures.Add("MinIOSettings.BucketName is required");

            if (failures.Any())
                return ValidateOptionsResult.Fail(failures);

            return ValidateOptionsResult.Success;
        }
    }
}
