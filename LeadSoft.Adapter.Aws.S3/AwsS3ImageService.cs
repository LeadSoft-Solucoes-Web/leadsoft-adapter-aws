using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace LeadSoft.Adapter.Aws.S3
{
    public sealed class AwsS3ImageService : IAwsS3ImageService, IDisposable
    {
        private readonly IAmazonS3 _s3;
        private bool disposedValue;

        public AwsS3ImageService(IConfiguration cfg)
        {
            string accessKey = cfg["S3:AccessKey"];
            string secretKey = cfg["S3:SecretAccessKey"];
            string region = cfg["S3:Region"];

            if (string.IsNullOrWhiteSpace(accessKey) ||
                string.IsNullOrWhiteSpace(secretKey) ||
                string.IsNullOrWhiteSpace(region))
            {
                throw new InvalidOperationException("Config S3 incompleta. Defina S3:AccessKey, S3:SecretAccessKey, e S3:Region.");
            }

            _s3 = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.GetBySystemName(region));
        }

        /// <summary>
        /// Método para upload de imagem para o S3.
        /// </summary>
        /// <param name="bucketName">Nome do Bucket</param>
        /// <param name="file">IForm file</param>
        /// <param name="filename">Nome do arquivo</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Key da imagem publicada</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public async Task<string> UploadFileAsync(string bucketName, IFormFile file, string filename, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length == 0)
                throw new ArgumentException("Arquivo vazio.", nameof(file));

            await using var stream = file.OpenReadStream();

            PutObjectRequest put = new()
            {
                BucketName = bucketName,
                Key = string.Format(filename, Path.GetExtension(file.FileName)),
                InputStream = stream,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.Private,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            put.Metadata["uploaded-at"] = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture);

            try
            {
                PutObjectResponse resp = await _s3.PutObjectAsync(put, ct);

                if ((int)resp.HttpStatusCode >= 300)
                    throw new IOException($"Falha no upload S3 (HTTP {(int)resp.HttpStatusCode}).");

                return put.Key;
            }
            catch (Exception ex)
            {
                throw new IOException("Erro ao fazer upload para o S3.", ex);
            }
        }

        /// <summary>
        /// Obtenha uma URL pré-assinada para acessar a imagem no S3.
        /// </summary>
        /// <param name="bucketName">Nome do Bucket</param>
        /// <param name="key">Key da imagem</param>
        /// <param name="expiresIn">Tempo desejado para expiração.</param>
        /// <returns>URL da imagem obtida pela Key.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> GetPresignedUrlAsync(string bucketName, string key, TimeSpan? expiresIn = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            GetPreSignedUrlRequest req = new()
            {
                BucketName = bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromMinutes(10))
            };

            req.ResponseHeaderOverrides.ContentType = req.ContentType;

            return await Task.FromResult(_s3.GetPreSignedURL(req));
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _s3.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~S3ImageService()
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
