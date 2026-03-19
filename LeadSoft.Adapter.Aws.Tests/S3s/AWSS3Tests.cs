using LeadSoft.Adapter.Aws.S3;
using LeadSoft.Adapter.Aws.Tests.S3s.Fixtures;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Reflection;

namespace LeadSoft.Adapter.Aws.Tests.S3s
{
    public class AWSS3Tests(AppSettingsFixture fx) : IClassFixture<AppSettingsFixture>
    {
        private readonly IAwsS3ImageService _S3ImgService = new AwsS3ImageService(fx.Configuration);

        [Fact]
        public async Task ImageUpload()
        {
            IFormFile formFile = await CreatePngAsync();

            try
            {
                string key = await _S3ImgService.UploadFileAsync(_BucketName, formFile, GetFilePath(Guid.NewGuid().GetString(), Path.GetRandomFileName()));
                Assert.True(key.IsSomething());
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
        }

        [Theory]
        [InlineData("staging/2025/10/5/ytdhauln.qwr.jpg")]
        public async Task ImageRead(string key)
        {
            try
            {
                string url = await _S3ImgService.GetPresignedUrlAsync(_BucketName, key);
                Assert.True(url.IsSomething());
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
        }

        /// <summary>
        /// S3 File Key format
        /// </summary>
        /// <remarks>
        /// 0- Environment
        /// 1- Year
        /// 2- Month
        /// 3- UserId
        /// 4- Filename
        /// 5- Fileextension
        /// </remarks>
        private const string _S3FileKey = "{0}{1}/{2}/{3}/{4}{5}";
        private const string _BucketName = "seu-bucket";

        public static string GetFilePath(string userId, string filename)
        {
            bool isProduction = false;
            DateTime now = DateTime.Now.Date;

            return string.Format(_S3FileKey,
                                 isProduction ? string.Empty : $"{EnvUtil.Get(EnvUtil.AspNet)}/",
                                 now.Year, now.Month, userId, filename, "{0}");
        }

        private static async Task<FormFile> CreatePngAsync()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetEmbeddedResourceStream("LeadSoft.FTeam.ECX.Tests.AWS_Toolkit.Aws_S3.Samples.LeadSoft.png");

            return new FormFile(stream, 0, stream.Length, "file", Path.GetFileName("LeadSoft.png"))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }
    }
}
