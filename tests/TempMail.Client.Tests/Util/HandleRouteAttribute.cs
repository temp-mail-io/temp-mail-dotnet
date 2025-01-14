using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TempMail.Client.Tests.Util;

[AttributeUsage(AttributeTargets.Method)]
internal class HandlerAttribute(
    [StringSyntax(StringSyntaxAttribute.Regex)] string route,
    string httpMethod) 
    : Attribute
{
    private readonly Regex _regex = new(route, RegexOptions.Compiled);
    
    public string Route { get; } = route;

    public string? Matches(HttpRequestMessage request)
    {
        var match = _regex.Match(request.RequestUri?.ToString() ?? string.Empty);
        if (!match.Success ||
            !request.Method.Method.Equals(httpMethod, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }
        
        return match.Groups.Count > 1
            ? match.Groups[1].Value
            : string.Empty;
    }
}