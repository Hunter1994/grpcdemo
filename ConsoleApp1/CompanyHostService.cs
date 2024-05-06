using Google.Protobuf;
using Grpc.Core;
using GrpcService1.Protos;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.Company;

namespace ConsoleApp1
{
    public class CompanyHostService : IHostedService
    {
        private CompanyClient _companyClient;
        private Uploader.UploaderClient _uploaderClient;
        const int ChunkSize = 1024 * 32; // 32 KB


        public CompanyHostService(Company.CompanyClient companyClient,Uploader.UploaderClient uploaderClient)
        {
            _companyClient = companyClient;
            _uploaderClient = uploaderClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //var result = await _companyClient.GetAsync(new CompanyRequest());
            //Console.WriteLine(JsonFormatter.Default.Format(result.Data));

            var call = _uploaderClient.UploadFile();
            await call.RequestStream.WriteAsync(new UploadFileRequest()
            {
                Metadata = new FileMetadata()
                {
                    FileName = "pancakes.jpg"
                }
            });

            var buffer = new byte[ChunkSize];
            using var readStream = File.OpenRead("pancakes.jpg");
            while (true)
            {
                var count = await readStream.ReadAsync(buffer);
                if (count == 0) break;
                await call.RequestStream.WriteAsync(new UploadFileRequest()
                {
                    Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, count))
                });
            }

            await call.RequestStream.CompleteAsync();

            var responsecall = await call;
            Console.WriteLine(responsecall.Id);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
