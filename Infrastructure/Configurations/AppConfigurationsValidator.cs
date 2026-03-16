using Domain.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Configurations
{
    public class AppConfigurationsValidator : IValidateOptions<AppConfigurations>
    {
        public ValidateOptionsResult Validate(string? name, AppConfigurations options)
        {
            var failures = new List<string>();

            if (options.MaxFileSizeMB <= 0)
                failures.Add("AppConfigurations.MaxFileSizeMB must be greater than zero");

            if (string.IsNullOrWhiteSpace(options.AllowedMimes))
                failures.Add("AppConfigurations.AllowedMimes is required");

            if (options.MaxJobRetry < 0)
                failures.Add("AppConfigurations.MaxJobRetry cannot be negative");

            if (options.PollingIntervalSeconds < 10)
                failures.Add("AppConfigurations.PollingIntervalSeconds cannot be less than 10");

            if (failures.Any())
                return ValidateOptionsResult.Fail(failures);

            return ValidateOptionsResult.Success;
        }
    }
}
