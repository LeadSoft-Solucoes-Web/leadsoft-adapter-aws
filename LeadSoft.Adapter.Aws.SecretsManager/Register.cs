using Microsoft.Extensions.DependencyInjection;

namespace LeadSoft.Adapter.Aws.SecretsManager
{
    /// <summary>
    /// Provides extension methods for registering AWS Secrets Manager services with a dependency injection container.
    /// </summary>
    /// <remarks>This class contains static methods to facilitate the integration of AWS Secrets Manager into
    /// applications using dependency injection. Use these methods during application startup to configure service
    /// registration.</remarks>
    public static class Register
    {
        /// <summary>
        /// Adds the AWS Secrets Manager service to the specified service collection for dependency injection.
        /// </summary>
        /// <remarks>This method registers the IAwsSecretManager interface with a singleton lifetime. Call
        /// this method during application startup to enable AWS Secrets Manager integration via dependency
        /// injection.</remarks>
        /// <param name="services">The service collection to which the AWS Secrets Manager service will be added. Cannot be null.</param>
        public static void AddAwsSecretManagerService(this IServiceCollection services)
        {
            services.AddSingleton<IAwsSecretManager, AwsSecretManager>();
        }
    }
}
