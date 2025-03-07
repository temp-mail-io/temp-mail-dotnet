﻿using Microsoft.Extensions.DependencyInjection;

namespace TempMail.Client.AspNetCore;

/// <summary>
/// Contains helper methods for registering <see cref="TempMailClient"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register <see cref="TempMailClient"/>
    /// </summary>
    public static IServiceCollection AddTempMailClient(
        this IServiceCollection services,
        string? apiKey = default)
    {
        services
            .AddHttpClient(nameof(TempMailClient))
            .Services.AddSingleton(_ => TempMailClientConfigurationBuilder
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

    /// <summary>
    /// Register <see cref="TempMailClient"/>
    /// </summary>
    public static IServiceCollection AddTempMailClient(
        this IServiceCollection services,
        TempMailClientConfiguration config)
    {
        services
            .AddHttpClient(nameof(TempMailClient))
            .Services.AddSingleton(_ => config)
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