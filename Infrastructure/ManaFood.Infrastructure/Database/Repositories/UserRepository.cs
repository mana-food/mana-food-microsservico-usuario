using ManaFood.Domain.Entities;
using ManaFood.Application.Interfaces;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace ManaFood.Infrastructure.Database.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IAmazonDynamoDB dynamoDbClient, IDynamoDBContext context) 
    : base(dynamoDbClient, context) { }
}
