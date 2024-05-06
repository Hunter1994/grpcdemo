using Grpc.Core;
using GrpcService1.Protos;
using Microsoft.Extensions.Configuration;
namespace GrpcService1.Services
{
    public class UploderService: Uploader.UploaderBase
    {
        private IConfiguration _configuration;

        public UploderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            var uploadId = Path.GetRandomFileName();
            var uploadPath = Path.Combine(_configuration["StoredFilesPath"], uploadId);
            Directory.CreateDirectory(uploadPath);
            using var writeStream = File.Create(Path.Combine(uploadPath, "data.bin"));
            await foreach (var message in requestStream.ReadAllAsync())
            {
                if (message.Metadata != null)
                {
                    await File.WriteAllTextAsync(Path.Combine(uploadPath, "metadata.json"), message.Metadata.ToString());
                }
                if (message.Data != null)
                {
                    await writeStream.WriteAsync(message.Data.Memory);
                }
            }
            return new UploadFileResponse()
            {
                Id = uploadId
            };
        }
    }
}
