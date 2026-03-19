using Microsoft.Extensions.DependencyInjection;

namespace LeadSoft.Adapter.Aws.S3
{
    /// <summary>
    /// Provides extension methods for registering AWS S3 image services with a dependency injection container.
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// Adds the AWS S3 image service to the specified service collection for dependency injection.
        /// </summary>
        /// <remarks>This method registers the IAwsS3ImageService interface with its default
        /// implementation, AwsS3ImageService, using a scoped lifetime. Call this method during application startup to
        /// enable AWS S3 image operations via dependency injection.</remarks>
        /// <param name="services">The service collection to which the AWS S3 image service will be added. Cannot be null.</param>
        public static void AddAwsS3ImageService(this IServiceCollection services)
        {
            services.AddScoped<IAwsS3ImageService, AwsS3ImageService>();
        }
    }
}
