using Microsoft.Extensions.DependencyInjection;

namespace TempMail.Client.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTempMailClient(
        this IServiceCollection services,
        string? apiKey = default)
    {
        services
            .AddHttpClient(nameof(TempMailClient))
            .Services.AddSingleton<TempMailClientConfiguration>(_ => TempMailClientConfigurationBuilder
                .Create()
                .WithApiKey(apiKey)
                .Build())
            .AddTransient<ITempMailClient>(p =>
            {
                var httpClientFactory = p.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(TempMailClient));
                var configuration = p.GetRequiredService<TempMailClientConfiguration>();
                return TempMailClient.Create(configuration, httpClient);
            });
        
        return services;
    }
    public static IServiceCollection AddTempMailClient(
        this IServiceCollection services,
        TempMailClientConfiguration config)
    {
        services
            .AddHttpClient(nameof(TempMailClient))
            .Services.AddSingleton<TempMailClientConfiguration>(_ => config)
            .AddTransient<ITempMailClient>(p =>
            {
                var httpClientFactory = p.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(TempMailClient));
                var configuration = p.GetRequiredService<TempMailClientConfiguration>();
                return TempMailClient.Create(configuration, httpClient);
            });
        
        return services;
    }
}