using System;
using System.Reflection;

namespace TempMail.Client;

/// <summary>
/// <see cref="TempMailClientConfiguration"/> builder
/// </summary>
public class TempMailClientConfigurationBuilder
{
    private TempMailClientConfigurationBuilder() { }

    private TempMailClientConfigurationBuilder(TempMailClientConfiguration config)
    {
        configuration = config;
    }
    private readonly TempMailClientConfiguration configuration = null!;

    public static TempMailClientConfigurationBuilder Create()
    {
        return new TempMailClientConfigurationBuilder(new TempMailClientConfiguration());
    }

    /// <summary>
    /// Specify API-key
    /// </summary>
    public TempMailClientConfigurationBuilder WithApiKey(string? apiKey) =>
        new(configuration with { ApiKey = apiKey! });

    /// <summary>
    /// Specify API URL, useful for testing
    /// </summary>
    public TempMailClientConfigurationBuilder WithApiUrl(string apiUrl) =>
        new(configuration with { ApiUrl = apiUrl });

    /// <summary>
    /// Specify the OS version, can be used for tests
    /// </summary>
    public TempMailClientConfigurationBuilder WithOsVersion(string osVersion) =>
        new(configuration with { OsVersion = osVersion });

    /// <summary>
    /// Specify the .NET Runtime, can be used for tests
    /// </summary>
    public TempMailClientConfigurationBuilder WithDotnetRuntimeVersion(string dotnetRuntimeVersion) =>
        new(configuration with { DotnetRuntimeVersion = dotnetRuntimeVersion });

    /// <summary>
    /// Specify the <c>TempMail.Client</c> library version, can be used for tests
    /// </summary>
    public TempMailClientConfigurationBuilder WithClientVersion(string clientVersion) =>
        new(configuration with { ClientVersion = clientVersion });

    /// <summary>
    /// Build configuration
    /// </summary>
    /// <returns><see cref="TempMailClientConfiguration"/></returns>
    /// <exception cref="ArgumentException">Thrown if the <see cref="TempMailClientConfiguration.ApiKey"/> is not specified</exception>
    public TempMailClientConfiguration Build()
    {

        var url = Environment.GetEnvironmentVariable("TEMP_MAIL_API_URL");
        if (string.IsNullOrEmpty(url))
        {
            url = string.IsNullOrEmpty(configuration.ApiUrl)
                ? "https://api.temp-mail.io"
                : configuration.ApiUrl;
        }
        configuration.ApiUrl = url;

        var apiKey = Environment.GetEnvironmentVariable("TEMP_MAIL_API_KEY");
        if (string.IsNullOrEmpty(apiKey) &&
            string.IsNullOrEmpty(configuration.ApiKey))
        {
            throw new ArgumentException("API Key is missing.");
        }
        configuration.ApiKey = apiKey ?? configuration.ApiKey;

        if (string.IsNullOrEmpty(configuration.OsVersion))
        {
            configuration.OsVersion = Environment.OSVersion.VersionString;
        }

        if (string.IsNullOrEmpty(configuration.DotnetRuntimeVersion))
        {
            configuration.DotnetRuntimeVersion = Environment.Version.ToString();
        }

        if (string.IsNullOrEmpty(configuration.ClientVersion))
        {
            configuration.ClientVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        }

        return configuration;
    }
}