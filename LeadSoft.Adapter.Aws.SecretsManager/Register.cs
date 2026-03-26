using Amazon.SecurityToken.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// Adds the AWS Secrets Manager service to the specified service collection for dependency injection.
        /// </summary>
        /// <remarks>Registers the IAwsSecretManager implementation as a singleton in the dependency injection container. This method should be called during application startup to enable secret retrieval from AWS Secrets Manager.</remarks>
        /// <param name="services">The service collection to which the AWS Secrets Manager service will be added. Cannot be null.</param>
        /// <param name="assumeRole">An optional AWS AssumeRoleRequest used to assume a specific IAM role when accessing secrets. If null, the default credentials are used.</param>
        /// <param name="logger">An optional logger instance for logging operations performed by the AWS Secrets Manager service.</param>
        public static void AddAwsSecretManagerService(this IServiceCollection services, AssumeRoleRequest? assumeRole = null, ILogger<AwsSecretManager>? logger = null)
        {
            services.AddSingleton<IAwsSecretManager>(awsSecretManager => new AwsSecretManager(assumeRole, logger));
        }
    }
}
