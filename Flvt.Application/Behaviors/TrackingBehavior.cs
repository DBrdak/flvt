using System.Diagnostics;
using Flvt.Domain.Primitives.Responses;
using MediatR;
using Serilog;
using Serilog.Context;

namespace Flvt.Application.Behaviors;

public sealed class TrackingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty("CorrelationId", $"{Guid.NewGuid()}"))
        {
            Log.Logger.Information("Started processing {request}", typeof(TRequest).Name);
            var timer = new Stopwatch();

            timer.Start();

            try
            {
                return await next();
            }
            catch (Exception e)
            {
                Log.Logger.Error(
                    "An error occurred while processing {request}: {error}",
                    typeof(TRequest).Name,
                    e);

                return Error.Exception as TResponse;
            }
            finally
            {
                timer.Stop();

                Log.Logger.Information(
                    "Processing {request} took {time}ms",
                    typeof(TRequest).Name,
                    timer.ElapsedMilliseconds);

                await Log.CloseAndFlushAsync();
            }
        }
    }
}