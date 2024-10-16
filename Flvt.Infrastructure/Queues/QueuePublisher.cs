using Flvt.Domain.Primitives.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Amazon.SQS;
using Flvt.Application.Abstractions;
using Flvt.Infrastructure.Utlis.Extensions;

namespace Flvt.Infrastructure.Queues;

internal class QueuePublisher : IQueuePublisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IConfiguration _configuration;

    public QueuePublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        _sqsClient = new AmazonSQSClient();
    }

    public async Task<Result> PublishFinishedBatches(CancellationToken cancellationToken)
    {
        var queueName = _configuration["queueNames:finishedBatches"] ??
                        throw new ArgumentNullException("queueNames:finishedBatches");

        return await PublishMessageAsync(queueName, null, cancellationToken);
    }

    private async Task<Result> PublishMessageAsync(string queueName, object? message, CancellationToken cancellationToken)
    {
        var getQueueUrlResult = await GetQueueAsync(queueName, cancellationToken);

        if (getQueueUrlResult.IsFailure)
        {
            return getQueueUrlResult.Error;
        }

        var queueUrl = getQueueUrlResult.Value;
        var messageJson = JsonConvert.SerializeObject(message);

        var response = await _sqsClient.SendMessageAsync(queueUrl, messageJson, cancellationToken);

        return !response.HttpStatusCode.IsSuccessStatusCode() ?
            QueueMessagePublisherErrors.ConnectionError :
            Result.Success();
    }

    private async Task<Result<string>> GetQueueAsync(string queueName, CancellationToken cancellationToken)
    {
        var queueGetRespone = await _sqsClient.GetQueueUrlAsync(queueName, cancellationToken);

        return !queueGetRespone.HttpStatusCode.IsSuccessStatusCode() ?
            QueueMessagePublisherErrors.ConnectionError :
            queueGetRespone.QueueUrl;
    }
}