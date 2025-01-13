using TempMail.Client.Models;

namespace TempMail.Client.Responses;

public class GetAvailableDomainsResponse(Domain[] domains)
{
    public Domain[] Domains { get; } = domains;
}