using ManaFood.Application.Interfaces;
using ManaFood.Infrastructure.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon;

namespace ManaFood.Infrastructure.Configurations;

public static class ServiceExtensions
{
    public static void ConfigurePersistenceApp(this IServiceCollection services)
    {
        var region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
        
        services.AddSingleton<IAmazonDynamoDB>(sp =>
        {
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(region)
            };
            return new AmazonDynamoDBClient(config);
        });

        services.AddSingleton<IDynamoDBContext>(sp =>
        {
            var client = sp.GetRequiredService<IAmazonDynamoDB>();
            var config = new DynamoDBContextConfig
            {
                ConsistentRead = false,
                SkipVersionCheck = false
            };
            return new DynamoDBContext(client, config);
        });

        services.AddScoped<IUserRepository, UserRepository>();
    }
}