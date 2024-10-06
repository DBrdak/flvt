using Flvt.Processor.AI.GPT;
using Microsoft.Extensions.DependencyInjection;

namespace Flvt.Processor;

public static class ProcessorInjector
{
    public static IServiceCollection AddProcessor(this IServiceCollection services)
    {
        return services;
    }

    private static void AddAIProcessor(this IServiceCollection services)
    {
        services.Configure<GPTOptions>();

        services.AddTransient<GeminiDelegatingHandler>();

        services.AddHttpClient<GeminiClient>(
                (serviceProvider, httpClient) =>
                {
                    var geminiOptions = serviceProvider.GetRequiredService<IOptions<GeminiOptions>>().Value;

                    httpClient.BaseAddress = new Uri(geminiOptions.Url);
                })
            .AddHttpMessageHandler<GeminiDelegatingHandler>();

        services.AddScoped<ILLMService, LLMService>();
    }
}