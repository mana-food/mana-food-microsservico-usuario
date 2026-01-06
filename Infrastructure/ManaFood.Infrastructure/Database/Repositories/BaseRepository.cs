using System.Linq.Expressions;
using ManaFood.Application.Shared;
using ManaFood.Domain.Entities;
using ManaFood.Application.Interfaces;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;


namespace ManaFood.Infrastructure.Database.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly IAmazonDynamoDB _dynamoDbClient;
    protected readonly IDynamoDBContext _context;
    public BaseRepository(IAmazonDynamoDB dynamoDbClient, IDynamoDBContext context)
    {
        _dynamoDbClient = dynamoDbClient;
        _context = context; 
    }

    public async Task<List<T>> GetAll(CancellationToken cancellationToken)
    {
        var search = _context.ScanAsync<T>(new List<ScanCondition>
        {
            new ScanCondition("Deleted", ScanOperator.Equal, false)
        });

        var items = await search.GetRemainingAsync(cancellationToken);
        return items;
    }

    public async Task<Paged<T>> GetAllPaged(int page, int pageSize, CancellationToken cancellationToken)
    {
        var search = _context.ScanAsync<T>(new List<ScanCondition>
        {
            new ScanCondition("Deleted", ScanOperator.Equal, false)
        });

        var allItems = await search.GetRemainingAsync(cancellationToken);
        var totalCount = allItems.Count;

        var pagedData = allItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new Paged<T>
        {
            Data = pagedData,
            TotalCount = totalCount
        };
    }
    
    public async Task<T?> GetBy(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includes)
    {
        var search = _context.ScanAsync<T>(new List<ScanCondition>());
        var items = await search.GetRemainingAsync(cancellationToken);
        
        var compiled = predicate.Compile();
        return items.FirstOrDefault(compiled);
    }

    public async Task<List<T>> GetByIds(List<Guid> ids, CancellationToken cancellationToken)
    {
        var batchGet = _context.CreateBatchGet<T>();
        
        foreach (var id in ids)
        {
            batchGet.AddKey(id);
        }

        await batchGet.ExecuteAsync(cancellationToken);
        
        return batchGet.Results.Where(x => !x.Deleted).ToList();
    }

    public async Task<T> Create(T entity, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        entity.Id = Guid.NewGuid();
        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        entity.Deleted = false;

        await _context.SaveAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<T> Update(T entity, CancellationToken cancellationToken)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveAsync(entity, cancellationToken);
        return entity;
    }

    public async Task Delete(T entity, CancellationToken cancellationToken)
    {
        entity.Deleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveAsync(entity, cancellationToken);
    }
}
