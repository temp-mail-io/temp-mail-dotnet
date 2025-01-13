namespace TempMail.Client.Requests;

public class GetMessageSourceCodeRequest
{
    private GetMessageSourceCodeRequest(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public static GetMessageSourceCodeRequest Create(string id) => new(id);
}