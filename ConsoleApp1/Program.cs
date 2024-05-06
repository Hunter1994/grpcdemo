using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ConsoleApp1;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcService1.Protos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Client;
using Shared.Contracts;



var defaultMethodConfig = new MethodConfig
{
    Names = { MethodName.Default },
    RetryPolicy = new RetryPolicy
    {
        MaxAttempts = 5,//最大重试次数为 5 次。
        InitialBackoff = TimeSpan.FromSeconds(1),//初始退避时间为 1 秒。
        MaxBackoff = TimeSpan.FromSeconds(5),//最大退避时间为 5 秒。
        BackoffMultiplier = 1.5,//退避时间的倍增因子为 1.5 倍。
        RetryableStatusCodes = { StatusCode.Unavailable }//可重试的状态码集合，这里包含了 Unavailable 状态码，表示服务不可用。
    }
};


var connectionFactory = new NamedPipesConnectionFactory("MyPipeName");
var socketsHttpHandler = new SocketsHttpHandler
{
    ConnectCallback = connectionFactory.ConnectAsync
};



using var channel = GrpcChannel.ForAddress("https://localhost:7258", new GrpcChannelOptions()
{
    //HttpHandler = socketsHttpHandler
});
var client = channel.CreateGrpcService<IEmployeeService>();
var res=await client.SayHelloAsync(new RequestDto() { Name = "1" });
Console.WriteLine(res.Message);

var health=new Health.HealthClient(channel);
var healthRes=await health.CheckAsync(new HealthCheckRequest());

var host = Host.CreateDefaultBuilder();
host.ConfigureServices(options => {
    //options.AddGrpcClient<Company.CompanyClient>(o => o.Address = new Uri("https://localhost:7258"));
    //options.AddGrpcClient<Uploader.UploaderClient>(o => o.Address = new Uri("http://localhost"));
    options.AddGrpcClient<Downloader.DownloaderClient>(o => o.Address = new Uri("https://localhost:7258"))
    .ConfigureChannel(options =>
    {
        options.ServiceConfig = new Grpc.Net.Client.Configuration.ServiceConfig
        {
            MethodConfigs = { defaultMethodConfig }
        };
        //options.HttpHandler = socketsHttpHandler;
    });


}).ConfigureServices(context => {
    //context.AddHostedService<CompanyHostService>();
    context.AddHostedService<DownloaderHostService>();
});

await host.Build().RunAsync();


//using var channal= GrpcChannel.ForAddress("https://localhost:7258");

//var client =new Greeter.GreeterClient(channal);
//var reply=await client.SayHelloAsync(new HelloRequest
//{
//    Name = "GreeterClient"
//});

//Console.WriteLine(reply.Message);


//var result = client.StreamingFromServer(new HelloRequest() { Name = "clientaaaaaaaaaaaaabbbbbbbbbbbbbbcccccccccc" });
//await foreach (var item in result.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine(item.Message);
//}

//var res = client.StreamingFromClient();
//for (int i = 0; i < 5; i++)
//{
//    await res.RequestStream.WriteAsync(new HelloRequest() { Name = i.ToString() });
//}
//await res.RequestStream.CompleteAsync();
//var aa = await res;
//Console.WriteLine(aa.Message);


//var ds = client.DownloadResults();
//int index = 0;
//Task.Run(async () => {
//    while (true)
//    {
//        await ds.RequestStream.WriteAsync(new HelloRequest() { Name = index++.ToString() });
//        await Task.Delay(1000);
//    }
//});

//Task.Run(async () => {
//while (true)
//{
//    await foreach (var item in ds.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine(item);
//}
//    }
//});


//var companyRpc = new Company.CompanyClient(channal);
//var cResult =await companyRpc.GetAsync(new CompanyRequest() { Name = "aa" });

//var json= JsonFormatter.Default.Format(cResult.Data);

Console.ReadLine();

