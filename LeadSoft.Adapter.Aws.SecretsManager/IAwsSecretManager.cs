using LeadSoft.Common.Library.Exceptions;

namespace LeadSoft.Adapter.Aws.SecretsManager
{
    /// <summary>
    /// Defines a singleton interface for interacting with AWS Secrets Manager.
    /// </summary>
    /// <remarks>This interface provides a method to retrieve secret values from AWS Secrets Manager
    /// asynchronously. Implementations of this interface are expected to handle the retrieval process and any
    /// associated errors.</remarks>
    public interface IAwsSecretManager : IDisposable
    {
        /// <summary>
        /// Asynchronously retrieves the secret value associated with the specified key.
        /// </summary>
        /// <remarks>
        /// This method retrieves sensitive information securely from a secret store.
        /// Ensure that the caller has appropriate permissions to access the secret store and handle the returned value securely.
        /// </remarks>
        /// <param name="aKey">The key used to identify the secret value. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the secret value as a string.</returns>
        Task<string> GetSecretValueAsync(string aKey);

        /// <summary>
        /// Asynchronously retrieves the names of all available secret keys.
        /// </summary>
        /// <returns>A list of strings containing the names of all secret keys. The list will be empty if no secrets are
        /// available.</returns>
        /// <exception cref="ForbiddenAppException">Thrown if the method is called in a production environment, where listing secret key names is not permitted.</exception>
        /// <exception cref="BadRequestAppException">Thrown if an error occurs while retrieving the secret values.</exception>
        Task<IList<string>> GetSecretsKeyNamesAsync();
    }
}
