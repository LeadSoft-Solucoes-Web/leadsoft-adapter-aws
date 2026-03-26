using Amazon.SecurityToken.Model;
using LeadSoft.Adapter.Aws.SecretsManager;
using LeadSoft.Adapter.Aws.Tests.SecretsManagers.Fixtures;
using LeadSoft.Common.Library.EnvUtils;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Xunit.Abstractions;

namespace LeadSoft.Adapter.Aws.Tests.SecretsManagers
{
    [Collection("EnvVar collection")]
    public class AWSSecretsManager_Tests(EnvVarFixture envVarFixture, ITestOutputHelper output) : IClassFixture<EnvVarFixture>
    {
        private readonly ILogger<AwsSecretManager> logger = new Logger<AwsSecretManager>(new LoggerFactory());

        [Fact]
        public async Task GetSecretNamesAsync()
        {
            IAwsSecretManager aws = new AwsSecretManager();

            IList<string> keys = await aws.GetSecretsKeyNamesAsync();
            output.WriteLine(string.Join(Environment.NewLine, keys));
        }

        [Fact]
        public async Task GetSecretNamesWithCredentialsAsync()
        {
            AssumeRoleRequest assumeRoleRequest = new()
            {
                RoleArn = EnvUtil.Get("AWS_SECRETS_MANAGER_ROLE_ARN"),
                RoleSessionName = EnvUtil.Get("AWS_SECRETS_MANAGER_ROLE_SESSION_NAME"),
            };

            IAwsSecretManager aws = new AwsSecretManager(assumeRoleRequest, logger);

            IList<string> keys = await aws.GetSecretsKeyNamesAsync();
            output.WriteLine(string.Join(Environment.NewLine, keys));
        }

        [Theory]
        [InlineData("TestSecretKey")]
        public async Task GetSecretValueAsync(string secretKey)
        {
            IAwsSecretManager aws = new AwsSecretManager();

            string secret = await aws.GetSecretValueAsync(secretKey);
            output.WriteLine(secret);
        }

        [Theory]
        [InlineData("TestSecretKey")]
        public async Task GetSecretValueWithCredentialsAsync(string secretKey)
        {
            AssumeRoleRequest assumeRoleRequest = new()
            {
                RoleArn = EnvUtil.Get("AWS_SECRETS_MANAGER_ROLE_ARN"),
                RoleSessionName = EnvUtil.Get("AWS_SECRETS_MANAGER_ROLE_SESSION_NAME")
            };

            IAwsSecretManager aws = new AwsSecretManager(assumeRoleRequest, logger);

            string secret = await aws.GetSecretValueAsync(secretKey);
            output.WriteLine(secret);
        }


        [Fact]
        public async Task GetRSA()
        {
            RSA rSA = RSA.Create();

            output.WriteLine("Public: " + Convert.ToBase64String(rSA.ExportRSAPublicKey()));
            output.WriteLine("Private: " + Convert.ToBase64String(rSA.ExportRSAPrivateKey()));
        }
    }
}
