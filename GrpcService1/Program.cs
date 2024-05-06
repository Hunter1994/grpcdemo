using GrpcService1.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using ProtoBuf.Grpc.Server;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);


//var socketPath = Path.Combine(Path.GetTempPath(), "socket.tmp");
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenNamedPipe("MyPipeName", listenOptions =>
//    {
//        listenOptions.Protocols = HttpProtocols.Http2;
//    });
//});
builder.Services.AddGrpcReflection();
builder.Services.AddGrpcHealthChecks()
                .AddCheck("Sample", () => HealthCheckResult.Healthy());

builder.Services.AddCodeFirstGrpc();

// Add services to the container.
builder.Services.AddGrpc(options => { 
    options.EnableDetailedErrors = true;
}).AddJsonTranscoding();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "GrpcService1.xml");
    c.IncludeXmlComments(filePath);
    c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
});



builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.UseGrpcWeb(new GrpcWebOptions()
{
    DefaultEnabled = true
});
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().RequireCors("AllowAll");
app.MapGrpcService<CompanyService>().RequireCors("AllowAll");
app.MapGrpcService<UploderService>();
app.MapGrpcService<DownloaderService>();
app.MapGrpcService<EmployeeService>();
app.MapGrpcHealthChecksService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");



app.Run();
