namespace TempMail.Client;

public record TempMailClientConfiguration
{
    internal TempMailClientConfiguration()
    {
    }
    
    public string ApiUrl { get; internal set; }
    public string ApiKey { get; internal set; }
    public string OsVersion { get; internal set; }
    public string DotnetRuntimeVersion { get; internal set; }
    public string ClientVersion { get; internal set; }
}