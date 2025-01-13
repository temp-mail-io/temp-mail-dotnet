namespace TempMail.Client.Requests;

public class GetAllMessagesRequest
{
    private GetAllMessagesRequest(string email)
    {
        Email = email;
    }
    public string Email { get; }

    public static GetAllMessagesRequest Create(string email)
    {
        Helpers.ValidateEmail(email);
        
        return new GetAllMessagesRequest(email);
    }
}