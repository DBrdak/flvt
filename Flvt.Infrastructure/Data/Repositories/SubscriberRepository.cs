using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Data.DataModels;
using Flvt.Infrastructure.Data.DataModels.Subscribers;
using Flvt.Infrastructure.Messanger.Emails.Models;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class SubscriberRepository : Repository<Subscriber>, ISubscriberRepository
{
    public SubscriberRepository(
        DataContext context,
        DataModelService<Subscriber> dataModelService) : base(
        context,
        dataModelService)
    {
    }

    public async Task<Result<Subscriber>> GetByEmailAsync(string email) => await GetByIdAsync(email);

    public async Task<Result<IEnumerable<Subscriber>>> GetManyByEmailAsync(IEnumerable<string> emails) => 
        await GetManyByIdAsync(emails);
}