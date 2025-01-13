using System;
using System.Text.Json.Serialization;

namespace TempMail.Client.Models;

/// <summary>
/// Rate limit status of the currently used API-key
/// </summary>
/// <param name="limit">Overall requests limit</param>
/// <param name="remaining">Remaining requests count</param>
/// <param name="used">Already used requests count</param>
/// <param name="reset">Timestamp at which the currently used API-key rate limit will be reset. Specified in seconds since UNIX epoch</param>
public class RateLimitStatus(
    int limit,
    int remaining,
    int used,
    int reset)
{
    /// <summary>
    /// Overall requests limit
    /// </summary>
    public int Limit { get; } = limit;
    
    /// <summary>
    /// Remaining requests count
    /// </summary>
    public int Remaining { get; } = remaining;
    
    /// <summary>
    /// Already used requests count
    /// </summary>
    public int Used { get; } = used;
    
    /// <summary>
    /// Timestamp at which the currently used API-key rate limit will be reset. Represented in seconds since UNIX epoch
    /// </summary>
    public int Reset { get; } = reset;
    
    /// <summary>
    /// Timestamp at which the currently used API-key rate limit will be reset. Represented as <see cref="DateTime"/> <br/>
    /// Will be ignored in JSON serialization with <c>System.Text.Json</c>
    /// </summary>
    [JsonIgnore]
    public DateTime ResetDateTime => DateTime.UnixEpoch.AddSeconds(Reset); 
}