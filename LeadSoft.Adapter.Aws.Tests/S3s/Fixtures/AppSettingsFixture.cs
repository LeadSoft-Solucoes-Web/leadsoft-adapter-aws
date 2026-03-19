using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace LeadSoft.Adapter.Aws.Tests.S3s.Fixtures
{
    public sealed class AppSettingsFixture : IDisposable
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public AppSettingsFixture()
        {
            string json =
                @"
                {
                  ""S3"": {
                    ""AccessKey"": """",
                    ""SecretAccessKey"": """",
                    ""Region"": """",
                  }
                }
                ";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            Configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();

            HostEnvironment = new TestHostEnvironment();
        }

        public sealed class TestHostEnvironment(string applicationName = "Tests", string contentRootPath = ".") : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = Environments.Staging;
            public string ApplicationName { get; set; } = applicationName;
            public string ContentRootPath { get; set; } = contentRootPath;
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }

        public void Dispose()
        {
        }
    }
}
