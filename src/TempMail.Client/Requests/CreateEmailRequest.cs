using TempMail.Client.Models;

namespace TempMail.Client.Requests;

/// <summary>
/// Request for temporary e-mail address creation. <br/>
/// </summary>
/// <remarks>
/// Can be one of: <br/>
/// <list type="bullet">
/// <item><see cref="CreateEmailByEmailRequest"/> - will be created an e-mail box with specified address</item>
/// <item><see cref="CreateEmailByDomainRequest"/> - will be created an e-mail box with random address on specified domain</item>
/// <item><see cref="CreateEmailByDomainTypeRequest"/> - will be created an e-mail box with random address on random domain of specified <see cref="DomainType"/></item>
/// </list>
/// </remarks>
public abstract class CreateEmailRequest
{
    /// <summary>
    /// Create <see cref="CreateEmailByEmailRequest"/>
    /// </summary>
    /// <param name="email">Specific e-mail address with which the box will be created</param>
    /// <returns><see cref="CreateEmailByEmailRequest"/></returns>
    public static CreateEmailRequest ByEmail(string email)
    {
        Helpers.ValidateEmail(email);

        return new CreateEmailByEmailRequest(email);
    }

    /// <summary>
    /// Create <see cref="CreateEmailByDomainRequest"/>
    /// </summary>
    /// <param name="domain">Specific domain on which the box will be created</param>
    /// <returns><see cref="CreateEmailByDomainRequest"/></returns>
    public static CreateEmailRequest ByDomain(string domain)
    {
        Helpers.ValidateDomain(domain);

        return new CreateEmailByDomainRequest(domain);
    }

    /// <summary>
    /// Create <see cref="CreateEmailByDomainTypeRequest"/>
    /// </summary>
    /// <param name="domainType">Specific <see cref="DomainType"/> of which the box will be created</param>
    /// <returns><see cref="CreateEmailByDomainTypeRequest"/></returns>
    public static CreateEmailRequest ByDomainType(DomainType domainType)
    {
        Helpers.ValidateDomainType(domainType);
        
        return new CreateEmailByDomainTypeRequest(domainType);
    }
}

/// <summary>
/// Request to create an e-mail box with specified address <br/>
/// Can be created with <see cref="CreateEmailRequest.ByEmail"/>
/// </summary>
public class CreateEmailByEmailRequest : CreateEmailRequest
{
    internal CreateEmailByEmailRequest(string email)
    {
        Email = email;
    }

    /// <summary>
    /// Specific e-mail address with which the box will be created
    /// </summary>
    public string Email { get; }
}

/// <summary>
/// Request to create an e-mail box with random address on specified domain <br/>
/// Can be created with <see cref="CreateEmailRequest.ByDomain"/>
/// </summary>
public class CreateEmailByDomainRequest : CreateEmailRequest
{
    internal CreateEmailByDomainRequest(string domain)
    {
        Domain = domain;
    }

    /// <summary>
    /// Specific domain on which the box will be created
    /// </summary>
    public string Domain { get; }
}

/// <summary>
/// Request to create an e-mail box with random address on random domain of specified <see cref="DomainType"/> <br/>
/// Can be created with <see cref="CreateEmailRequest.ByDomainType"/>
/// </summary>
public class CreateEmailByDomainTypeRequest : CreateEmailRequest
{
    internal CreateEmailByDomainTypeRequest(DomainType domainType)
    {
        DomainType = domainType;
    }

    /// <summary>
    /// Specific <see cref="DomainType"/> of which the box will be created
    /// </summary>
    public DomainType DomainType { get; }
}