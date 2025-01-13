namespace TempMail.Client.Requests;

/// <summary>
/// Request to delete the e-mail box
/// </summary>
public class DeleteEmailRequest
{
    private DeleteEmailRequest(string email)
    {
        Email = email;
    }
    
    /// <summary>
    /// E-mail address to delete
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Create <see cref="DeleteEmailRequest"/>
    /// </summary>
    /// <param name="email">E-mail address to delete</param>
    /// <returns><see cref="DeleteEmailRequest"/></returns>
    public static DeleteEmailRequest Create(string email)
    {
        Helpers.ValidateEmail(email);
        
        return new DeleteEmailRequest(email);
    }
}