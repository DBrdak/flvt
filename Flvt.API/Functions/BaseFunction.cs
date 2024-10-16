
using Amazon.Lambda.Core;
using MediatR;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Flvt.API.Functions;

public abstract class BaseFunction
{
    protected readonly ISender Sender;

    protected BaseFunction(ISender sender)
    {
        Sender = sender;
    }
}