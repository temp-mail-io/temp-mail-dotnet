namespace TempMail.Client.Requests;

/// <summary>
/// Get specific e-mail message
/// </summary>
public class GetSpecificMessageRequest
{
    private GetSpecificMessageRequest(string id)
    {
        Id = id;
    }

    /// <summary>
    /// ID of the e-mail message to get
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Create <see cref="GetSpecificMessageRequest"/>
    /// </summary>
    /// <param name="id">ID of the e-mail message to get</param>
    /// <returns><see cref="GetSpecificMessageRequest"/></returns>
    public static GetSpecificMessageRequest Create(string id) => new(id);
}