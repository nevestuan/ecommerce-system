using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using ProductEngine.Application.Interfaces;
using ProductEngine.Application.Services;
using ProductEngine.Infrastructure.Repositories;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Cosmos DB configuration
builder.Services.AddSingleton(s =>
{
    var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")!;
    return new CosmosClient(connectionString);
});

builder.Services.AddSingleton<IProductRepository>(s =>
{
    var cosmosClient = s.GetRequiredService<CosmosClient>();
    var databaseId = Environment.GetEnvironmentVariable("CosmosDbDatabaseId")!;
    var containerId = Environment.GetEnvironmentVariable("CosmosDbContainerId")!;
    return new ProductRepository(cosmosClient, databaseId, containerId);
});

builder.Services.AddSingleton<IProductService, ProductService>();

builder.Build().Run();
