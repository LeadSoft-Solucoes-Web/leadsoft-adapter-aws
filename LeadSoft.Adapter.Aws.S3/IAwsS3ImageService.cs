using Microsoft.AspNetCore.Http;

namespace LeadSoft.Adapter.Aws.S3
{
    public interface IAwsS3ImageService
    {
        /// <summary>
        /// Faz upload de um arquivo JPG/JPEG para o S3 e retorna a chave gerada.
        /// Em ambiente de homologação, prefixa "staging/" como primeira pasta.
        /// </summary>
        Task<string> UploadFileAsync(string bucketName, IFormFile file, string filename, CancellationToken ct = default);

        /// <summary>
        /// Gera uma URL pré-assinada temporária para a chave informada.
        /// </summary>
        Task<string> GetPresignedUrlAsync(string bucketName, string key, TimeSpan? expiresIn = null);
    }
}
