namespace TempMail.Client.Responses;

public class CreateEmailResponse(string email, int ttl)
{
    public string Email { get; } = email;
    public int Ttl { get; } = ttl;
}