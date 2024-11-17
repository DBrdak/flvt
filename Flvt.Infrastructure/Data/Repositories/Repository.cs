using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Data.DataModels;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Data.Repositories;

internal abstract class Repository<TEntity>
{
    protected readonly Table Table;
    protected readonly DataContext Context;
    private DocumentBatchWrite? _batchWrite;
    private DocumentBatchGet? _batchGet;
    private readonly DataModelService<TEntity> _dataModelService;

    protected Repository(DataContext context, DataModelService<TEntity> dataModelService)
    {
        Context = context;
        _dataModelService = dataModelService;
        Table = context.Set<TEntity>();
    }

    public virtual async Task<Result<IEnumerable<TEntity>>> GetAllAsync()
    {
        var scanner = Table.Scan(new ScanFilter());
        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (!scanner.IsDone);

        var records = docs.Select(_dataModelService.ConvertDocumentToDataModel);
        var entities = records.Select(record => record.ToDomainModel());

        return Result.Create(entities);
    }

    protected virtual async Task<Result<IEnumerable<Document>>> GetAllAsync(
        int? limit = null,
        List<string>? attributesToGet = null)
    {
        var config = new ScanOperationConfig();

        if (limit.HasValue)
        {
            config.Limit = limit.Value;
        }

        if (attributesToGet is not null)
        {
            config.AttributesToGet = attributesToGet;
            config.Select = SelectValues.SpecificAttributes;
        }

        var scanner = Table.Scan(config);

        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (docs.Count <= limit || !scanner.IsDone);

        return Result.Create(docs.Take(limit ?? docs.Count));
    }

    protected virtual async Task<Result<IEnumerable<TEntity>>> GetWhereAsync(ScanFilter filter)
    {
        var scanner = Table.Scan(filter);

        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (!scanner.IsDone);

        var records = docs.Select(_dataModelService.ConvertDocumentToDataModel);
        var entities = records.Select(record => record.ToDomainModel());

        return Result.Create(entities);
    }

    protected virtual async Task<Result<IEnumerable<TEntity>>> GetWhereAsync(
        ScanFilter filter,
        int? limit = null)
    {
        var config = new ScanOperationConfig
        {
            Filter = filter,
        };

        if (limit.HasValue)
        {
            config.Limit = limit.Value;
        }

        var scanner = Table.Scan(config);

        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (docs.Count <= limit || !scanner.IsDone);

        var records = docs.Select(_dataModelService.ConvertDocumentToDataModel);
        var entities = records.Select(record => record.ToDomainModel()).ToList();

        return Result.Create(entities.Take(limit ?? entities.Count));
    }

    protected virtual async Task<Result<IEnumerable<Document>>> GetWhereAsync(
        ScanFilter filter,
        int? limit = null,
        List<string>? attributesToGet = null)
    {
        var config = new ScanOperationConfig
        {
            Filter = filter,
        };

        if (limit.HasValue)
        {
            config.Limit = limit.Value;
        }

        if (attributesToGet is not null)
        {
            config.AttributesToGet = attributesToGet;
            config.Select = SelectValues.SpecificAttributes;
        }

        var scanner = Table.Scan(config);

        var docs = new List<Document>();

        do
            docs.AddRange(await scanner.GetNextSetAsync());
        while (docs.Count <= limit || !scanner.IsDone);

        return Result.Create(docs.Take(limit ?? docs.Count));
    }


    public async Task<Result<TEntity>> GetByIdAsync(string id)
    {
        var doc = await Table.GetItemAsync(new Primitive(id));

        if (doc is null)
        {
            return Error.NotFound<TEntity>();
        }

        var record = _dataModelService.ConvertDocumentToDataModel(doc);
        var entity = record.ToDomainModel();

        return Result.Create(entity);
    }

    public async Task<Result<IEnumerable<TEntity>>> GetManyByIdAsync(IEnumerable<string> ids)
    {
        var batch = Table.CreateBatchGet();
        
        ids.ToList().ForEach(id => batch.AddKey(new Primitive(id)));

        await batch.ExecuteAsync();

        var docs = batch.Results;

        var records = docs.Select(_dataModelService.ConvertDocumentToDataModel);
        var entities = records.Select(record => record.ToDomainModel());

        return Result.Create(entities);
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
        var record = _dataModelService.ConvertDomainModelToDataModel(entity);
        var json = JsonConvert.SerializeObject(record);
        var doc = Document.FromJson(json);

        await Table.PutItemAsync(doc);

        return Result.Success(entity);
    }

    public async Task<Result> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var batch = Table.CreateBatchWrite();

        var records = entities.Select(_dataModelService.ConvertDomainModelToDataModel);
        var jsons = records.Select(JsonConvert.SerializeObject);
        var docs = jsons.Select(Document.FromJson);

        docs.ToList().ForEach(batch.AddDocumentToPut);

        await batch.ExecuteAsync();

        return Result.Success();
    }

    public async Task<Result<TEntity>> UpdateAsync(
        TEntity entity)
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

        var record = _dataModelService.ConvertDomainModelToDataModel(entity);
        var json = JsonConvert.SerializeObject(record);
        var doc = Document.FromJson(json);

        _batchWrite.AddDocumentToPut(doc);
    }

    public void AddManyItemsToBatchWrite(IEnumerable<TEntity> entities)
    {
        if (_batchWrite is null)
        {
            throw new InvalidOperationException("Batch write was not started");
        }

        var records = entities.Select(_dataModelService.ConvertDomainModelToDataModel);
        var jsons = records.Select(JsonConvert.SerializeObject);
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

        _batchGet.AddKey(id);
    }

    public void AddManyItemsToBatchGet(IEnumerable<string> ids)
    {
        if (_batchGet is null)
        {
            throw new InvalidOperationException("Batch get was not started");
        }

        ids.ToList().ForEach(id => _batchGet.AddKey(id));
    }

    public async Task<Result<IEnumerable<TEntity>>> ExecuteBatchGetAsync()
    {
        if (_batchGet is null)
        {
            throw new InvalidOperationException("Batch get was not started");
        }

        await _batchGet.ExecuteAsync();

        var docs = _batchGet.Results;

        var records = docs.Select(_dataModelService.ConvertDocumentToDataModel);
        var entities = records.Select(record => record.ToDomainModel());

        return Result.Create(entities);
    }
}