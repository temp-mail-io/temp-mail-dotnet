namespace TempMail.Client.Requests;

public class DeleteEmailRequest
{
    private DeleteEmailRequest(string email)
    {
        Email = email;
    }
    public string Email { get; }

    public static DeleteEmailRequest Create(string email)
    {
        Helpers.ValidateEmail(email);
        
        return new DeleteEmailRequest(email);
    }
}