using System.Net.Http.Headers;
using System.Text;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches.Create;

internal sealed class BatchFileFactory
{
    public static ByteArrayContent CreateJsonlFile(IEnumerable<BatchMessageLine> batchMessages)
    {
        var tasks = string.Join('\n', batchMessages.Select(GPTJsonConvert.Serialize));

        var fileBytes = Encoding.UTF8.GetBytes(tasks);
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        return fileContent;
    }
}