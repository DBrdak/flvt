using System.Net;
using Serilog;

namespace Flvt.Infrastructure.Utlis.Extensions;

internal static class HttpClientExtensions
{
    private static readonly Task<HttpResponseMessage> errorResponse =
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));

    public static Task<HttpResponseMessage> TryPostAsync(
        this HttpClient client,
        string requestUri,
        HttpContent content)
    {
        try
        {
            return client.PostAsync(requestUri, content);
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Error occured when tried to call: [POST]{uri}, error: {error}, stack trace: {stack trace}",
                requestUri,
                e.Message,
                e.StackTrace);

            return errorResponse;
        }
    }

    public static Task<HttpResponseMessage> TryGetAsync(
        this HttpClient client,
        string requestUri)
    {
        try
        {
            return client.GetAsync(requestUri);
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Error occured when tried to call: [GET]{uri}, error: {error}, stack trace: {stack trace}",
                requestUri,
                e.Message,
                e.StackTrace);

            return errorResponse;
        }
    }

    public static Task<HttpResponseMessage> TryPutAsync(
        this HttpClient client,
        string requestUri,
        HttpContent content)
    {
        try
        {
            return client.PutAsync(requestUri, content);
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Error occured when tried to call: [PUT]{uri}, error: {error}, stack trace: {stack trace}",
                requestUri,
                e.Message,
                e.StackTrace);

            return errorResponse;
        }
    }

    public static Task<HttpResponseMessage> TryDeleteAsync(
        this HttpClient client,
        string requestUri)
    {
        try
        {
            return client.DeleteAsync(requestUri);
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Error occured when tried to call: [DELETE]{uri}, error: {error}, stack trace: {stack trace}",
                requestUri,
                e.Message,
                e.StackTrace);

            return errorResponse;
        }
    }
}