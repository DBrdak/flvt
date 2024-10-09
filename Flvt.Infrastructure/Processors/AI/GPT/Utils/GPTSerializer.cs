using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal static class GPTJsonConvert
{
    private static readonly JsonSerializerSettings serializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    public static string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, serializerSettings);
    public static T? DeserializeObject<T>(string json) => JsonConvert.DeserializeObject<T>(json, serializerSettings);
}