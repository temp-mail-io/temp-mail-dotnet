namespace TempMail.Client.Responses;

/// <summary>
/// Response on the <see cref="TempMail.Client.Requests.CreateEmailRequest"/>
/// </summary>
/// <param name="email">The created e-mail box address</param>
/// <param name="ttl">The created e-mail box's Time To Live</param>
public class CreateEmailResponse(string email, int ttl)
{
    /// <summary>
    /// The created e-mail box address
    /// </summary>
    public string Email { get; } = email;
    
    /// <summary>
    /// The created e-mail box's Time To Live
    /// </summary>
    public int Ttl { get; } = ttl;
}