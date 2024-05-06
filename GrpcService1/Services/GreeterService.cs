using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService1;
using System.Threading.Channels;

namespace GrpcService1.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
        public override async Task StreamingFromServer(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            for (var i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new HelloReply() { Message = i.ToString() });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            //DecimalValue decimalValue = new();
            //decimalValue.Units = 1;
            //decimalValue.Nanos = 500_000_000;

            //decimal a = decimalValue * 2;  //1.5*2 = 3.0


            //var channel = Channel.CreateBounded<HelloReply>(new BoundedChannelOptions(5));
            //var consumerTask = Task.Run(async () =>
            //{
            //    await foreach (var item in channel.Reader.ReadAllAsync())
            //    {
            //        await responseStream.WriteAsync(item);
            //    }
            //});

            //var dataChunks = request.Name.Chunk(10);
            //await Task.WhenAll(dataChunks.Select(async c =>
            //{
            //    string str = string.Join("", c);

            //    var helloReply = new HelloReply()
            //    {
            //        Message = str,
            //        Duration = Duration.FromTimeSpan(TimeSpan.FromSeconds(1)),
            //        Start = Timestamp.FromDateTime(DateTime.Now),
            //        Detail = Any.Pack(new Person() { Name = "zs" })
            //    };


            //    await channel.Writer.WriteAsync(helloReply); 


            //}));

            //channel.Writer.Complete();
            //await consumerTask;
        }

        public override async Task<HelloReply> StreamingFromClient(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            await foreach (var item in requestStream.ReadAllAsync())
            {
                Console.WriteLine(item.Name);
            }
            return new HelloReply() { Message = "³É¹¦" };
        }
        public override async Task DownloadResults(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            Task request = Task.Run(async () =>
            {
                await foreach (var item in requestStream.ReadAllAsync())
                {
                    Console.WriteLine(item.Name);
                }
            });

            Task request2 = Task.Run(async () =>
            {
                await foreach (var item in requestStream.ReadAllAsync())
                {
                    Console.WriteLine(item.Name);
                }
            });

            int index = 0;

            while (!request.IsCompleted)
            {
                await responseStream.WriteAsync(new HelloReply { Message = index.ToString() });
                await Task.Delay(1000);
            }
           
        }
    }
}
