using Google.Protobuf;
using Grpc.Core;
using GrpcService1.Protos;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrpcService1.Protos.Downloader;

namespace ConsoleApp1
{
    public class DownloaderHostService : IHostedService
    {
        const int ChunkSize = 1024 * 32; // 32 KB
        private DownloaderClient _downloaderClient;

        public DownloaderHostService(Downloader.DownloaderClient  downloaderClient)
        {
            _downloaderClient = downloaderClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var downloadsPath = Path.Combine(Environment.CurrentDirectory, "downloads");
            var downloadId = Path.GetRandomFileName();
            var downloadIdPath = Path.Combine(downloadsPath, downloadId);
            Directory.CreateDirectory(downloadIdPath);


            using var call = _downloaderClient.DownloadFile(new DownloadFileRequest
            {
                Id = downloadId
            });

            await using var writeStream = File.Create(Path.Combine(downloadIdPath, "data.bin"));

            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                if (message.Metadata != null)
                {
                    Console.WriteLine("Saving metadata to file");
                    var metadata = message.Metadata.ToString();
                    await File.WriteAllTextAsync(Path.Combine(downloadIdPath, "metadata.json"), metadata);
                }
                if (message.Data != null)
                {
                    var bytes = message.Data.Memory;
                    Console.WriteLine($"Saving {bytes.Length} bytes to file");
                    await writeStream.WriteAsync(bytes);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
