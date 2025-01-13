using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TempMail.Sample.Console")]
namespace TempMail.Client;

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

    public TempMailClientConfigurationBuilder WithApiKey(string? apiKey) => 
        new(_configuration with { ApiKey = apiKey! });

    internal TempMailClientConfigurationBuilder WithApiUrl(string apiUrl) => 
        new(_configuration with { ApiUrl = apiUrl });

    public TempMailClientConfigurationBuilder WithOsVersion(string osVersion) => 
        new(_configuration with { OsVersion = osVersion });

    public TempMailClientConfigurationBuilder WithDotnetRuntimeVersion(string dotnetRuntimeVersion) => 
        new(_configuration with { DotnetRuntimeVersion = dotnetRuntimeVersion });

    public TempMailClientConfigurationBuilder WithClientVersion(string clientVersion) => 
        new(_configuration with { ClientVersion = clientVersion });

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