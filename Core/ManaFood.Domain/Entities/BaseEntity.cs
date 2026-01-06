using Amazon.DynamoDBv2.DataModel;

namespace ManaFood.Domain.Entities;

public abstract class BaseEntity
{
    [DynamoDBHashKey]
    public Guid Id { get; set; }
    
    [DynamoDBProperty]
    public DateTime CreatedAt { get; set; }
    
    [DynamoDBProperty]
    public Guid CreatedBy { get; set; }
    
    [DynamoDBProperty]
    public DateTime UpdatedAt { get; set; }
    
    [DynamoDBProperty]
    public Guid UpdatedBy { get; set; }
    
    [DynamoDBProperty]
    public bool Deleted { get; set; }
}