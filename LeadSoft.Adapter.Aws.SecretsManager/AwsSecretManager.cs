using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.Logging;

namespace LeadSoft.Adapter.Aws.SecretsManager
{
    /// <summary>
    /// Provides a singleton implementation for interacting with AWS Secrets Manager.
    /// </summary>
    /// <remarks>This class is designed to retrieve secret values from AWS Secrets Manager using a
    /// pre-configured request. It ensures that the appropriate secret is fetched based on the application's environment
    /// (e.g., production or pre-production). The class is intended to be used as a singleton to centralize secret
    /// management within the application.</remarks>
    public sealed class AwsSecretManager : IAwsSecretManager
    {
        private readonly AmazonSecretsManagerClient _AwsSM_Client;
        private readonly GetSecretValueRequest _AwsSM_Request;
        private readonly ILogger<AwsSecretManager> _Logger = new Logger<AwsSecretManager>(new LoggerFactory());
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AwsSecretManager"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the AWS Secrets Manager client and prepares a request to retrieve the secret.
        /// The secret ID is determined based on the current environment (production or pre-production), and the version stage is set to "AWSCURRENT".
        /// </remarks>
        public AwsSecretManager()
        {
            try
            {
                _AwsSM_Client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(EnvUtil.Get(EnvVariable.AwsSecretsManagerRegion)));

                _AwsSM_Request = new()
                {
                    SecretId = EnvUtil.Get(EnvVariable.AwsSecretsManagerArn),
                    VersionStage = "AWSCURRENT"
                };
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, $@"Error initializing AWS Secrets Manager client [{RegionEndpoint.GetBySystemName(EnvUtil.Get(EnvVariable.AwsSecretsManagerRegion))}, {EnvUtil.Get(EnvVariable.AwsSecretsManagerName)}, AWSCURRENT]: {ex.Message}");
                throw new BadRequestAppException($@"Error initializing AWS Secrets Manager client: {ex.Message}");
            }
        }


        /// <summary>
        /// Asynchronously retrieves the names of all available secret keys.
        /// </summary>
        /// <returns>A list of strings containing the names of all secret keys. The list will be empty if no secrets are
        /// available.</returns>
        /// <exception cref="ForbiddenAppException">Thrown if the method is called in a production environment, where listing secret key names is not permitted.</exception>
        /// <exception cref="BadRequestAppException">Thrown if an error occurs while retrieving the secret values.</exception>
        public async Task<IList<string>> GetSecretsKeyNamesAsync()
        {
            if (EnvUtil.IsProduction())
                throw new ForbiddenAppException($"You cannot list secrets key names from {EnvUtil.AspNet} environment.");

            try
            {
                Dictionary<string, string> secretValues = await GetSecretsAsync();

                return [.. secretValues.Select(kvp => kvp.Key)];
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                throw new BadRequestAppException($@"Error retrieving secret values: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously retrieves the secret value associated with the specified key.
        /// </summary>
        /// <remarks>
        /// This method retrieves sensitive information securely from a secret store.
        /// Ensure that the caller has appropriate permissions to access the secret store and handle the returned value securely.
        /// </remarks>
        /// <param name="aKey">The key used to identify the secret value. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the secret value as a string.</returns>
        public async Task<string> GetSecretValueAsync(string aKey)
        {
            try
            {
                Dictionary<string, string> secretValue = await GetSecretsAsync();

                if (!secretValue.TryGetValue(aKey, out string? secretValueString) || secretValueString is null)
                    throw new NotFoundAppException();

                return secretValueString;
            }
            catch (NotFoundAppException ex)
            {
                _Logger.LogError(ex, ex.Message);
                throw new NotFoundAppException($@"No found secret value for key ""{aKey}"": {ex.Message}");
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                throw new BadRequestAppException($@"Error retrieving secret value for ""{aKey}"": {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously retrieves a collection of secrets from AWS Secrets Manager.
        /// </summary>
        /// <remarks>This method fetches the secret value associated with the configured request and
        /// parses it into a dictionary of key-value pairs. The secret value is expected to be in JSON format.</remarks>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> containing the secrets as key-value pairs.</returns>
        /// <exception cref="BadRequestAppException">Thrown if an error occurs while retrieving the secret value.</exception>
        private async Task<Dictionary<string, string>> GetSecretsAsync()
        {
            try
            {
                GetSecretValueResponse response = await _AwsSM_Client.GetSecretValueAsync(_AwsSM_Request);

                return response.SecretString.JsonToObject<Dictionary<string, string>>().ToDictionary(
                    kvp => kvp.Key.ToUpper(),
                    kvp => kvp.Value
                );
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                throw new BadRequestAppException($@"Error retrieving secret values: {ex.Message}");
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _AwsSM_Client.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AwsSecretManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
