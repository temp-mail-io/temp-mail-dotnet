namespace TempMail.Client.Requests;

public class DeleteSpecificMessageRequest
{
    private DeleteSpecificMessageRequest(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public static DeleteSpecificMessageRequest Create(string id) => new(id);
}