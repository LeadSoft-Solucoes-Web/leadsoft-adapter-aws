using LeadSoft.Adapter.Aws.SecretsManager;
using LeadSoft.Adapter.Aws.Tests.SecretsManagers.Fixtures;
using System.Security.Cryptography;
using Xunit.Abstractions;

namespace LeadSoft.Adapter.Aws.Tests.SecretsManagers
{
    [Collection("EnvVar collection")]
    public class AWSSecretsManager_Tests(EnvVarFixture envVarFixture, ITestOutputHelper output) : IClassFixture<EnvVarFixture>
    {
        [Fact]
        public async Task GetSecretValueAsync()
        {
            IAwsSecretManager aws_singleton = new AwsSecretManager();

            string x = await aws_singleton.GetSecretValueAsync("CLIENT_ID");
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
