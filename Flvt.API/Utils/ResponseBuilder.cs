﻿using Amazon.Lambda.APIGatewayEvents;
using Flvt.Domain.Primitives.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Flvt.API.Utils;

public static class ResponseBuilder
{
    private static readonly DefaultContractResolver contractResolver = new()
    {
        NamingStrategy = new CamelCaseNamingStrategy
        {
            OverrideSpecifiedNames = false
        }
    };

    private static readonly JsonSerializerSettings serializerSettings =
        new()
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include
        };

    private static APIGatewayHttpApiV2ProxyResponse Respond(Result result, int statusCode, Dictionary<string, string>? headers = null) =>
        new()
        {
            StatusCode = result.Error == Error.Exception ? 500 : statusCode,
            Headers = headers ?? new Dictionary<string, string>(),
            Body = JsonConvert.SerializeObject(result, serializerSettings)
        };

    private static APIGatewayHttpApiV2ProxyResponse Respond<T>(Result<T> result, int statusCode, Dictionary<string, string>? headers = null) =>
        new()
        {
            StatusCode = result.Error == Error.Exception ? 500 : statusCode,
            Headers = headers ?? new Dictionary<string, string>(),
            Body = JsonConvert.SerializeObject(result, serializerSettings)
        };

    public static APIGatewayHttpApiV2ProxyResponse ReturnAPIResponse(
        this Result result,
        int? successStatusCode = null,
        int? failureStatusCode = null,
        Dictionary<string, string>? headers = null)
    {
        return result.IsSuccess ?
            Respond(result, successStatusCode ?? 200, headers) :
            Respond(result, failureStatusCode ?? 400, headers);
    }

    public static APIGatewayHttpApiV2ProxyResponse ReturnAPIResponse<T>(
        this Result<T> result,
        int? successStatusCode = null,
        int? failureStatusCode = null,
        Dictionary<string, string>? headers = null)
    {
        return result.IsSuccess ?
            Respond(result, successStatusCode ?? 200, headers) :
            Respond(result, failureStatusCode ?? 400, headers);
    }
}