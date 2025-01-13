using TempMail.Client.Models;

namespace TempMail.Client.Requests;

public abstract class CreateEmailRequest
{
    public static CreateEmailRequest ByEmail(string email)
    {
        Helpers.ValidateEmail(email);

        return new CreateEmailByEmailRequest(email);
    }

    public static CreateEmailRequest ByDomain(string domain)
    {
        Helpers.ValidateDomain(domain);

        return new CreateEmailByDomainRequest(domain);
    }

    public static CreateEmailRequest ByDomainType(DomainType domainType)
    {
        Helpers.ValidateDomainType(domainType);
        
        return new CreateEmailByDomainTypeRequest(domainType);
    }
}

public class CreateEmailByEmailRequest(string email) : CreateEmailRequest
{
    public string Email => email;
}

public class CreateEmailByDomainRequest(string domain) : CreateEmailRequest
{
    public string Domain => domain;
}

public class CreateEmailByDomainTypeRequest(DomainType domainType) : CreateEmailRequest
{
    public DomainType DomainType => domainType;
}