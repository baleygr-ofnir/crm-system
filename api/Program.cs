using api.Core.Services;
using Scalar.AspNetCore;
using api.Data.Entities;
using api.Endpoints;
using Microsoft.Azure.Cosmos;

namespace api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<CosmosClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var endpoint = configuration["CosmosDb:Endpoint"]
                           ?? throw new ArgumentNullException("CosmosDb__Endpoint is missing");
            var key = configuration["CosmosDb:Key"]
                      ?? throw new ArgumentNullException("CosmosDb__Key is missing");

            var options = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                HttpClientFactory = () =>
                {
                    var httpMessageHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    return new HttpClient(httpMessageHandler);
                }
            };

            return new CosmosClient(endpoint, key, options);
        });

        builder.Services.AddSingleton<Container>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var databaseName = configuration["CosmosDb:DatabaseName"]
                               ?? throw new ArgumentNullException("CosmosDb__DatabaseName is missing");
            var containerName = configuration["CosmosDb:ContainerName"]
                                ?? throw new ArgumentNullException("CosmosDb__ContainerName is missing");

            var client = sp.GetRequiredService<CosmosClient>();
            return client.GetContainer(databaseName, containerName);
        });

        builder.Services.AddScoped<CustomerService>();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var cosmosClient = services.GetRequiredService<CosmosClient>();

            string databaseName = configuration["CosmosDb:DatabaseName"] ??
                                  throw new ArgumentNullException("CosmosDb__DatabaseName is missing");
            string containerName = configuration["CosmosDb:ContainerName"] ??
                                   throw new ArgumentNullException("CosmosDb__ContainerName is missing");

            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            var database = databaseResponse.Database;

            await database.CreateContainerIfNotExistsAsync(containerName, "/id");
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapCustomerEndpoints();

        app.Run();
    }
}