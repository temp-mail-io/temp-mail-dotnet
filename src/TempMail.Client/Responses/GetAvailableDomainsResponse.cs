using TempMail.Client.Models;

namespace TempMail.Client.Responses;

/// <summary>
/// Response to available domains request/>
/// </summary>
/// <param name="domains">Available with currently used API-key list of <see cref="Domain"/>s</param>
public class GetAvailableDomainsResponse(Domain[] domains)
{
    /// <summary>
    /// Available with currently used API-key list of <see cref="Domain"/>s
    /// </summary>
    public Domain[] Domains { get; } = domains;
}