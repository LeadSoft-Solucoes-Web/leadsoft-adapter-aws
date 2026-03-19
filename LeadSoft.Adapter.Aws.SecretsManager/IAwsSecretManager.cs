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
    }
}
