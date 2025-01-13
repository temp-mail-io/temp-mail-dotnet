namespace TempMail.Client.Requests;

/// <summary>
/// Request to get all the messages in the e-mail box
/// </summary>
public class GetAllMessagesRequest
{
    private GetAllMessagesRequest(string email)
    {
        Email = email;
    }
    
    /// <summary>
    /// The box from which to get all the messages
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Create <see cref="GetAllMessagesRequest"/>
    /// </summary>
    /// <param name="email">The box from which to get all the messages</param>
    /// <returns><see cref="GetAllMessagesRequest"/></returns>
    public static GetAllMessagesRequest Create(string email)
    {
        Helpers.ValidateEmail(email);
        
        return new GetAllMessagesRequest(email);
    }
}