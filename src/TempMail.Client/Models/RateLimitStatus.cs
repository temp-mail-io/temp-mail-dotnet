using System;
using System.Text.Json.Serialization;

namespace TempMail.Client.Models;

public class RateLimitStatus(
    int limit,
    int remaining,
    int used,
    int reset)
{
    public int Limit { get; } = limit;
    public int Remaining { get; } = remaining;
    public int Used { get; } = used;
    public int Reset { get; } = reset;
    
    [JsonIgnore]
    public DateTime ResetDateTime => DateTime.UnixEpoch.AddSeconds(Reset); 
}