using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Data.Repositories;

internal abstract class Repository<TEntity>
{
    protected readonly Table Table;
    protected readonly DataContext Context;
    private DocumentBatchWrite? _batchWrite;
    private DocumentBatchGet? _batchGet;

    protected Repository(DataContext context)
    {
        Context = context;
        Table = context.Set<TEntity>();
    }

    public virtual async Task<Result<IEnumerable<TEntity>>> GetAllAsync()
    {
        var config = new ScanOperationConfig
        {
            Select = SelectValues.AllAttributes,
            Filter = new ScanFilter(),
            Limit = 1000,
        };

        var scanner = Table.Scan(config);
        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (!scanner.IsDone);

        var records = docs.Select(document => JsonConvert.DeserializeObject<TEntity>(document.ToJson()));

        return Result.Create(records);
    }

    protected virtual async Task<Result<IEnumerable<TEntity>>> GetWhereAsync(ScanFilter filter)
    {
        var scanner = Table.Scan(filter);

        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (!scanner.IsDone);

        var records = docs.Select(document => JsonConvert.DeserializeObject<TEntity>(document.ToJson()));

        return Result.Create(records);
    }


    public async Task<Result<TEntity>> GetByIdAsync(string id)
    {
        var doc = await Table.GetItemAsync(new Primitive(id));

        if (doc is null)
        {
            return Error.NotFound<TEntity>();
        }

        var record = JsonConvert.DeserializeObject<TEntity>(doc.ToJson());

        return record;
    }

    public async Task<Result<IEnumerable<TEntity>>> GetManyByIdAsync(IEnumerable<string> ids)
    {
        var batch = Table.CreateBatchGet();
        
        ids.ToList().ForEach(id => batch.AddKey(new Primitive(id)));

        await batch.ExecuteAsync();

        var docs = batch.Results;
        var records = docs.Select(document => JsonConvert.DeserializeObject<TEntity>(document.ToJson()));

        return Result.Create(records);
    }

    public async Task<Result> RemoveAsync(string entityId)
    {
        await Table.DeleteItemAsync(new Primitive(entityId));

        return Result.Success();
    }

    public async Task<Result> RemoveRangeAsync(IEnumerable<string> entitiesId)
    {
        var batch = Table.CreateBatchWrite();

        entitiesId.ToList().ForEach(id => batch.AddKeyToDelete(new Primitive(id)));

        await batch.ExecuteAsync();

        return Result.Success();
    }

    public async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        var json = JsonConvert.SerializeObject(entity);
        var doc = Document.FromJson(json);

        await Table.PutItemAsync(doc);

        return Result.Success(entity);
    }

    public async Task<Result> AddRangeVoidAsync(IEnumerable<TEntity> entities)
    {
        var batch = Table.CreateBatchWrite();

        var jsons = entities.Select(entity => JsonConvert.SerializeObject(entity));
        var docs = jsons.Select(Document.FromJson);

        docs.ToList().ForEach(batch.AddDocumentToPut);

        await batch.ExecuteAsync();

        return Result.Success();
    }

    public async Task<Result> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var batch = Table.CreateBatchWrite();

        var jsons = entities.Select(entity => JsonConvert.SerializeObject(entity));
        var docs = jsons.Select(Document.FromJson);

        docs.ToList().ForEach(batch.AddDocumentToPut);

        await batch.ExecuteAsync();

        return Result.Success();
    }

    public async Task<Result<TEntity>> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        return await AddAsync(entity);
    }

    public async Task<Result> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        return await AddRangeAsync(entities);
    }

    public void StartBatchWrite() => _batchWrite = Table.CreateBatchWrite();

    public void AddItemToBatchWrite(TEntity entity)
    {
        if (_batchWrite is null)
        {
            throw new InvalidOperationException("Batch write was not started");
        }

        var json = JsonConvert.SerializeObject(entity);
        var doc = Document.FromJson(json);

        _batchWrite.AddDocumentToPut(doc);
    }

    public void AddManyItemsToBatchWrite(IEnumerable<TEntity> entities)
    {
        if (_batchWrite is null)
        {
            throw new InvalidOperationException("Batch write was not started");
        }

        var jsons = entities.Select(e => JsonConvert.SerializeObject(e));
        var docs = jsons.Select(Document.FromJson).ToList();

        docs.ForEach(_batchWrite.AddDocumentToPut);
    }

    public async Task<Result> ExecuteBatchWriteAsync()
    {
        if (_batchWrite is null)
        {
            throw new InvalidOperationException("Batch write was not started");
        }

        await _batchWrite.ExecuteAsync();
        _batchWrite = null;

        return Result.Success();
    }

    public void StartBatchGet() => _batchGet = Table.CreateBatchGet();

    public void AddItemToBatchGet(string id)
    {
        if (_batchGet is null)
        {
            throw new InvalidOperationException("Batch get was not started");
        }

        _batchGet.AddKey(id);;
    }

    public async Task<Result<IEnumerable<TEntity>>> ExecuteBatchGetAsync()
    {
        if (_batchGet is null)
        {
            throw new InvalidOperationException("Batch get was not started");
        }

        await _batchGet.ExecuteAsync();

        var docs = _batchGet.Results;
        var records = docs.Select(document => JsonConvert.DeserializeObject<TEntity>(document.ToJson()));
        _batchGet = null;

        return Result.Create(records);
    }
}