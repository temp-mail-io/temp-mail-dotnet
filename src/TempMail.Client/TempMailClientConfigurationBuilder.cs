using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TempMail.Client;

/// <summary>
/// <see cref="TempMailClientConfiguration"/> builder
/// </summary>
public class TempMailClientConfigurationBuilder
{
    private TempMailClientConfigurationBuilder() { }

    private TempMailClientConfigurationBuilder(TempMailClientConfiguration config)
    {
        _configuration = config;
    }
    private readonly TempMailClientConfiguration _configuration;

    public static TempMailClientConfigurationBuilder Create()
    {
        return new TempMailClientConfigurationBuilder(new TempMailClientConfiguration());
    }

    /// <summary>
    /// Specify API-key
    /// </summary>
    public TempMailClientConfigurationBuilder WithApiKey(string? apiKey) => 
        new(_configuration with { ApiKey = apiKey! });

    /// <summary>
    /// Specify API URL, useful for testing
    /// </summary>
    public TempMailClientConfigurationBuilder WithApiUrl(string apiUrl) => 
        new(_configuration with { ApiUrl = apiUrl });

    /// <summary>
    /// Specify the OS version, can be used for tests
    /// </summary>
    public TempMailClientConfigurationBuilder WithOsVersion(string osVersion) => 
        new(_configuration with { OsVersion = osVersion });

    /// <summary>
    /// Specify the .NET Runtime, can be used for tests
    /// </summary>
    public TempMailClientConfigurationBuilder WithDotnetRuntimeVersion(string dotnetRuntimeVersion) => 
        new(_configuration with { DotnetRuntimeVersion = dotnetRuntimeVersion });

    /// <summary>
    /// Specify the <c>TempMail.Client</c> library version, can be used for tests
    /// </summary>
    public TempMailClientConfigurationBuilder WithClientVersion(string clientVersion) => 
        new(_configuration with { ClientVersion = clientVersion });

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
            url = string.IsNullOrEmpty(_configuration.ApiUrl) 
                ? "https://api.temp-mail.io"
                : _configuration.ApiUrl;
        }
        _configuration.ApiUrl = url;

        var apiKey = Environment.GetEnvironmentVariable("TEMP_MAIL_API_KEY");
        if (string.IsNullOrEmpty(apiKey) &&
            string.IsNullOrEmpty(_configuration.ApiKey))
        {
            throw new ArgumentException("API Key is missing.");
        }
        _configuration.ApiKey = apiKey ?? _configuration.ApiKey;

        if (string.IsNullOrEmpty(_configuration.OsVersion))
        {
            _configuration.OsVersion = Environment.OSVersion.VersionString;
        }

        if (string.IsNullOrEmpty(_configuration.DotnetRuntimeVersion))
        {
            _configuration.DotnetRuntimeVersion = Environment.Version.ToString();
        }

        if (string.IsNullOrEmpty(_configuration.ClientVersion))
        {
            _configuration.ClientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        return _configuration;
    }
}