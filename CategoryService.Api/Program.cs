using CategoryService.Api.Grpc.V1;
using CategoryService.Application.DependencyInjection;
using CategoryService.Infrastructure.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((ctx, services, config) =>
{
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("ServiceName", "CategoryService")
        .WriteTo.Async(a => a.Console(new JsonFormatter()),
            bufferSize: 10000,
            blockWhenFull: false)
        .WriteTo.Async(a => a.OpenTelemetry(opts =>
        {
            opts.Endpoint = builder.Configuration.GetConnectionString("OtlpEndpoint")
                ?? throw new InvalidOperationException("OtlpEndpoint is not configured");
            opts.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = "CategoryService"
            };
        }),
            bufferSize: 10000,
            blockWhenFull: false);
});

var app = builder.Build();

//await app.MigrateDatabaseAsync();
//await app.SeedDatabaseAsync();

app.UseHttpsRedirection();

app.MapGrpcService<GrpcServer>();
app.MapGrpcHealthChecksService();

app.Run();