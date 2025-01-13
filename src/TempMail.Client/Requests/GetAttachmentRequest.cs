namespace TempMail.Client.Requests;

public class GetAttachmentRequest(string id)
{
    public string Id { get; } = id;
}