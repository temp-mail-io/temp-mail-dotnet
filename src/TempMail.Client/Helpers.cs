using System;
using System.Text.RegularExpressions;
using TempMail.Client.Models;

namespace TempMail.Client;

public class Helpers
{
    public static readonly Regex EmailRegex = new (
        @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
    public static readonly Regex DomainRegex = new (
        @"^((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static void ValidateEmail(string email)
    {
        if (!EmailRegex.IsMatch(email))
        {
            throw new FormatException($"'{email}' is not a valid email address.");
        }
    }

    public static void ValidateDomain(string domain)
    {
        if (!DomainRegex.IsMatch(domain))
        {
            throw new FormatException($"'{domain}' is not a valid domain.");
        }
    }

    public static void ValidateDomainType(DomainType domainType)
    {
        if (!Enum.IsDefined(typeof(DomainType), domainType))
        {
            throw new FormatException($"'{domainType}' is not a valid domain type.");
        }
    }
}