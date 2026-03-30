using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SecurityToken.Model;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.Logging;

namespace LeadSoft.Adapter.Aws.SecretsManager
{
    /// <summary>
    /// Provides a singleton implementation for interacting with AWS Secrets Manager.
    /// </summary>
    /// <remarks>This class is designed to retrieve secret values from AWS Secrets Manager using a pre-configured request.
    /// It ensures that the appropriate secret is fetched based on the application's environment (e.g., production or pre-production). The class is intended to be used as a singleton to centralize secret management within the application.</remarks>
    public sealed class AwsSecretManager : IAwsSecretManager
    {
        private readonly AmazonSecretsManagerClient _AwsSM_Client;
        private readonly GetSecretValueRequest _AwsSM_Request;
        private readonly RegionEndpoint _AwsSM_Region = RegionEndpoint.GetBySystemName(EnvUtil.Get(EnvVariable.AwsSecretsManagerRegion));
        private bool disposedValue;
        private readonly ILogger<AwsSecretManager>? _Logger;

        /// <summary>
        /// Initializes a new instance of the AwsSecretManager class, optionally assuming an AWS role for credentials.
        /// </summary>
        /// <remarks>This constructor configures the AWS Secrets Manager client using either the provided
        /// role or default credentials. The client is initialized for the region and secret specified in the
        /// environment variables. Ensure that the required environment variables are set before instantiating this
        /// class.</remarks>
        /// <param name="assumeRole">An optional AssumeRoleRequest specifying the AWS role to assume for authentication. If null, the default
        /// credentials are used.</param>
        /// <param name="logger">An optional logger for logging errors and information. If null, logging is disabled.</param>
        /// <exception cref="BadRequestAppException">Thrown if the AWS Secrets Manager client cannot be initialized due to configuration or connection errors.</exception>
        public AwsSecretManager(AssumeRoleRequest? assumeRole = null, ILogger<AwsSecretManager>? logger = null)
        {
            _Logger = logger;
            try
            {
                _AwsSM_Client = assumeRole is not null ? new AmazonSecretsManagerClient(GetAWSSessionCredentialsAsync(assumeRole).Result, _AwsSM_Region)
                                                       : new AmazonSecretsManagerClient(_AwsSM_Region);

                _AwsSM_Request = new()
                {
                    SecretId = EnvUtil.Get(EnvVariable.AwsSecretsManagerArn),
                    VersionStage = "AWSCURRENT"
                };
            }
            catch (Exception ex)
            {
                _logErrorInitializingClient?.Invoke(_Logger, $"{RegionEndpoint.GetBySystemName(EnvUtil.Get(EnvVariable.AwsSecretsManagerRegion))}, {EnvUtil.Get(EnvVariable.AwsSecretsManagerName)}, AWSCURRENT", ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously obtains temporary AWS session credentials by assuming the specified role.
        /// </summary>
        /// <remarks>This method retrieves credentials from the AWS credential profile store using the
        /// provided role session name. Ensure that the credential profile is correctly configured before calling this
        /// method. If credentials cannot be retrieved, an exception is thrown.</remarks>
        /// <param name="assumeRole">The request containing the parameters required to assume the AWS role. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the session credentials for the
        /// assumed role, or null if the request is null.</returns>
        private async Task<SessionAWSCredentials> GetAWSSessionCredentialsAsync(AssumeRoleRequest assumeRole)
        {
            if (assumeRole is null)
                return null;
            try
            {
                if (!new CredentialProfileStoreChain().TryGetAWSCredentials(assumeRole.RoleSessionName, out AWSCredentials awsCredentials))
                    throw new Exception("Unable to retrieve AWS credentials from the credential profile store. Please ensure that the profile is correctly configured.");

                ImmutableCredentials credentials = await awsCredentials.GetCredentialsAsync();

                return new(credentials.AccessKey,
                           credentials.SecretKey,
                           credentials.Token);
            }
            catch (Exception ex)
            {
                _logErrorInitializingClient?.Invoke(_Logger, $"{RegionEndpoint.GetBySystemName(EnvUtil.Get(EnvVariable.AwsSecretsManagerRegion))}, {EnvUtil.Get(EnvVariable.AwsSecretsManagerName)}, AWSCURRENT", ex.Message, ex);
                throw;
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
                _logErrorRetrievingSecretValues?.Invoke(_Logger, ex);
                throw;
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
                _logNotFoundSecretValueForKey?.Invoke(_Logger, aKey, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logErrorRetrievingSecretValueForKey?.Invoke(_Logger, aKey, ex);
                throw;
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
                    kvp => kvp.Key.ToUpperInvariant(),
                    kvp => kvp.Value
                );
            }
            catch (Exception ex)
            {
                _logErrorRetrievingSecretValues?.Invoke(_Logger, ex);
                throw;
            }
        }

        #region [ Logging & Dispose pattern ]

        private static readonly Action<ILogger, string, string, Exception?> _logErrorInitializingClient =
            LoggerMessage.Define<string, string>(
                LogLevel.Error,
                new EventId(1, nameof(_logErrorInitializingClient)),
                "Error initializing AWS Secrets Manager client [{RegionAndName}]: {ErrorMessage}");

        private static readonly Action<ILogger, Exception?> _logErrorRetrievingSecretValues =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(2, nameof(_logErrorRetrievingSecretValues)),
                "Error retrieving secret values");

        private static readonly Action<ILogger, string, Exception?> _logErrorRetrievingSecretValueForKey =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(3, nameof(_logErrorRetrievingSecretValueForKey)),
                "Error retrieving secret value for \"{Key}\"");

        private static readonly Action<ILogger, string, Exception?> _logNotFoundSecretValueForKey =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(4, nameof(_logNotFoundSecretValueForKey)),
                "No found secret value for key \"{Key}\"");

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

        #endregion
    }
}
