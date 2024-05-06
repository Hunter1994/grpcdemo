using Google.Protobuf;
using Grpc.Core;
using GrpcService1.Protos;

namespace GrpcService1.Services
{
    public class DownloaderService: Downloader.DownloaderBase
    {
        private const int ChunkSize = 1024 * 32;
        public override async Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileResponse> responseStream, ServerCallContext context)
        {
            var requestParam = request.Id;
            var filename = requestParam switch
            {
                "4" => "pancakes4.png",
                _ => "pancakes.jpg",
            };

            await responseStream.WriteAsync(new DownloadFileResponse
            {
                Metadata = new DownloadFileMetadata { FileName = filename }
            });

            var buffer = new byte[ChunkSize];
            await using var fileStream = File.OpenRead(filename);

            while (true)
            {
                var numBytesRead = await fileStream.ReadAsync(buffer);
                if (numBytesRead == 0)
                {
                    break;
                }

                await responseStream.WriteAsync(new DownloadFileResponse
                {
                    Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, numBytesRead))
                });
            }
        }
    }
}
