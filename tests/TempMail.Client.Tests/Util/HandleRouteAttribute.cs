﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace TempMail.Client.Tests.Util;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
internal class HandlerAttribute(
    [StringSyntax(StringSyntaxAttribute.Regex)] string route,
    string httpMethod)
    : Attribute
{
    private readonly Regex regex = new(route, RegexOptions.Compiled);

    public string Route { get; } = route;

    public string HttpMethod { get; } = httpMethod;

    public string? Matches(HttpRequestMessage request)
    {
        var match = regex.Match(request.RequestUri?.ToString() ?? string.Empty);
        if (!match.Success ||
            !request.Method.Method.Equals(HttpMethod, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        if (match.Groups.Count > 1)
        {
            return match.Groups[1].Value.Contains('/')
                ? null
                : match.Groups[1].Value;
        }

        return string.Empty;
    }
}