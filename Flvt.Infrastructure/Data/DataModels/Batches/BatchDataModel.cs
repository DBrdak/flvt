using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.DataModels.Batches;

internal sealed record BatchDataModel : IDataModel<BatchDataModel>
{
    public string Id { get; init; }
    public IEnumerable<string> ProcessingAdvertisementsLinks { get; init; }
    public bool IsFinished { get; private set; }

    public BatchDataModel(string Id,
        IEnumerable<string> ProcessingAdvertisementsLinks,
        bool IsFinished = false)
    {
        this.Id = Id;
        this.ProcessingAdvertisementsLinks = ProcessingAdvertisementsLinks;
        this.IsFinished = IsFinished;
    }

    private BatchDataModel(Document document)
    {
        Id = document.GetProperty(nameof(Id));
        ProcessingAdvertisementsLinks = document.GetProperty(nameof(ProcessingAdvertisementsLinks)).AsArrayOfString();
        IsFinished = document.GetProperty(nameof(IsFinished)).AsBoolean();
    }

    public void FinishBatch() => IsFinished = true;

    public static BatchDataModel FromDomainModel(BatchDataModel domainModel) =>
        new(domainModel.Id, domainModel.ProcessingAdvertisementsLinks, domainModel.IsFinished);

    public static BatchDataModel FromDocument(Document document) => new(document);

    public Type GetDomainModelType() => typeof(BatchDataModel);

    public BatchDataModel ToDomainModel() =>
        new (Id, ProcessingAdvertisementsLinks, IsFinished);
}