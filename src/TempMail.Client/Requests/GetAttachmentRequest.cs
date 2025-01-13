namespace TempMail.Client.Requests;

/// <summary>
/// Request to get specific attachment from a message 
/// </summary>
public class GetAttachmentRequest
{
    private GetAttachmentRequest(string id)
    {
        Id = id;
    }

    /// <summary>
    /// ID of the attachment to get
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Create <see cref="GetAttachmentRequest"/>
    /// </summary>
    /// <param name="id">ID of the attachment to get</param>
    /// <returns><see cref="GetAttachmentRequest"/></returns>
    public static GetAttachmentRequest Create(string id) => new(id);
}