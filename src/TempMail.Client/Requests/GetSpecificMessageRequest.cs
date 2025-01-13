namespace TempMail.Client.Requests;

public class GetSpecificMessageRequest
{
    private GetSpecificMessageRequest(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public static GetSpecificMessageRequest Create(string id) => new(id);
}