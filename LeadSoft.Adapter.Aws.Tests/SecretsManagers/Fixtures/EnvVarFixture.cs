namespace LeadSoft.Adapter.Aws.Tests.SecretsManagers.Fixtures
{
    /// <summary>
    /// Provides a fixture for managing environment variables during application testing or execution.
    /// </summary>
    /// <remarks>This class sets specific environment variables required for the application, such as 
    /// "ASPNETCORE_ENVIRONMENT", "CIP_ClientId", and "CIP_ClientSecret". It ensures these variables  are properly
    /// initialized and provides cleanup functionality through the <see cref="Dispose"/> method. Use this fixture to
    /// manage environment variables in a controlled manner during tests or application setup.</remarks>
    public class EnvVarFixture : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvVarFixture"/> class and sets default environment variables.
        /// </summary>
        /// <remarks>Upon instantiation, this constructor sets the following environment variables to their default values: <list type="bullet">
        /// <item><description><c>ASPNETCORE_ENVIRONMENT</c> to <c>Development</c>.</description></item>
        /// <item><description><c>AWS_SECRETS_MANAGER_ARN</c> to an empty string.</description></item>
        /// <item><description><c>AWS_SECRETS_MANAGER_NAME</c> to an empty string.</description></item> </list> These values
        /// are typically used for configuring application behavior during development.</remarks>
        public EnvVarFixture()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("AWS_SECRETS_MANAGER_ARN", "");
            Environment.SetEnvironmentVariable("AWS_SECRETS_MANAGER_NAME", "");
            Environment.SetEnvironmentVariable("AWS_SECRETS_MANAGER_REGION", "");
        }

        /// <summary>
        /// Releases resources and resets environment variables used by the application.
        /// </summary>
        /// <remarks>This method is called when the fixture is disposed, allowing for cleanup of environment variables that were set during initialization. It resets the following environment variables to null: <list type="bullet">
        /// <item><description><c>ASPNETCORE_ENVIRONMENT</c></description></item>
        /// <item><description><c>AWS_SECRETS_MANAGER_ARN</c></description></item>
        /// <item><description><c>AWS_SECRETS_MANAGER_NAME</c></description></item>
        /// <item><description><c>AWS_SECRETS_MANAGER_REGION</c></description></item> </list></remarks>
        public void Dispose()
        {
            // Cleanup if needed
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("AWS_SECRETS_MANAGER_ARN", null);
            Environment.SetEnvironmentVariable("AWS_SECRETS_MANAGER_NAME", null);
            Environment.SetEnvironmentVariable("AWS_SECRETS_MANAGER_REGION", null);
        }
    }
}
