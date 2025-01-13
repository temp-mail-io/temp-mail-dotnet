namespace TempMail.Client;

/// <summary>
/// Configuration for the <see cref="TempMailClient"/>
/// </summary>
/// <remarks>Created via <see cref="TempMailClientConfigurationBuilder"/></remarks>
public record TempMailClientConfiguration
{
    internal TempMailClientConfiguration()
    {
    }
    
    /// <summary>
    /// API url, specifies address to which all the requests will be sent
    /// </summary>
    /// <remarks>May not be specified</remarks>
    public string ApiUrl { get; internal set; } = null!;

    /// <summary>
    /// API-key, secret used for authorization on the resource
    /// </summary>
    /// <remarks>Must be specified. Can be set via the <c>TEMP_MAIL_API_KEY</c> environment variable</remarks>
    public string ApiKey { get; internal set; } = null!;

    /// <summary>
    /// OS version, used for <c>User-Agent</c> header
    /// </summary>
    /// <remarks>May not be specified</remarks>
    public string OsVersion { get; internal set; } = null!;

    /// <summary>
    /// .NET Runtime version, used for <c>User-Agent</c> header
    /// </summary>
    /// <remarks>May not be specified</remarks>
    public string DotnetRuntimeVersion { get; internal set; } = null!;

    /// <summary>
    /// <c>TempMail.Client</c> library version, used for <c>User-Agent</c> header
    /// </summary>
    /// <remarks>May not be specified</remarks>
    public string ClientVersion { get; internal set; } = null!;
}