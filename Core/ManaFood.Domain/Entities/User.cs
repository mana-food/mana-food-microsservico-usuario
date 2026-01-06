using Amazon.DynamoDBv2.DataModel;
using ManaFood.Domain.Enums;

namespace ManaFood.Domain.Entities;

[DynamoDBTable("Users")]
public class User : BaseEntity
{
    [DynamoDBProperty]
    [DynamoDBGlobalSecondaryIndexHashKey("EmailIndex")]
    public required string Email { get; set; }
    
    [DynamoDBProperty]
    public required string Name { get; set; }
    
    [DynamoDBProperty]
    [DynamoDBGlobalSecondaryIndexHashKey("CpfIndex")]
    public required string Cpf { get; set; }
    
    [DynamoDBProperty]
    public required string Password { get; set; }
    
    [DynamoDBProperty]
    public DateOnly Birthday { get; set; }
    
    [DynamoDBProperty]
    public UserType UserType { get; set; }
}