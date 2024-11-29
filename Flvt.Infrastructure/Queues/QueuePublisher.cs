using Flvt.Domain.Primitives.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Amazon.SQS;
using Flvt.Application.Abstractions;
using Flvt.Domain.Filters;
using Flvt.Infrastructure.Utlis.Extensions;
using Serilog;
using Amazon.Runtime;
using Flvt.Domain;
using Flvt.Infrastructure.AWS.Contants;

namespace Flvt.Infrastructure.Queues;

internal class QueuePublisher : IQueuePublisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IConfiguration _configuration;

    public QueuePublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        _sqsClient = new AmazonSQSClient(new BasicAWSCredentials(XD.LOL2, XD.LOL3), CloudEnvironment.RegionEndpoint);
    }

    public async Task<Result> PublishFinishedBatches()
    {
        var queueName = _configuration["queueNames:finishedBatches"] ??
                        throw new ArgumentNullException("queueNames:finishedBatches");

        return await PublishMessageAsync(queueName, null);
    }

    public async Task<Result> PublishLaunchedFilters(List<Filter> launchedFilters)
    {
        var queueName = _configuration["queueNames:launchedFilters"] ??
                        throw new ArgumentNullException("queueNames:launchedFilters");

        var filtersIds = launchedFilters.Select(filter => filter.Id);

        return await PublishMessageAsync(queueName, filtersIds);
    }

    private async Task<Result> PublishMessageAsync(string queueName, object? message)
    {
        var getQueueUrlResult = await GetQueueAsync(queueName);

        if (getQueueUrlResult.IsFailure)
        {
            Log.Logger.Error("Failed to publish {queueName}", queueName);
            return getQueueUrlResult.Error;
        }

        var queueUrl = getQueueUrlResult.Value;
        var messageJson = JsonConvert.SerializeObject(message);

        var response = await _sqsClient.SendMessageAsync(queueUrl, messageJson);

        return !response.HttpStatusCode.IsSuccessStatusCode() ?
            QueueMessagePublisherErrors.ConnectionError :
            Result.Success();
    }

    private async Task<Result<string>> GetQueueAsync(string queueName)
    {
        var queueGetRespone = await _sqsClient.GetQueueUrlAsync(queueName);

        return !queueGetRespone.HttpStatusCode.IsSuccessStatusCode() ?
            QueueMessagePublisherErrors.ConnectionError :
            queueGetRespone.QueueUrl;
    }
}