namespace TempMail.Client.Requests;

/// <summary>
/// Get an e-mail message source code
/// </summary>
public class GetMessageSourceCodeRequest
{
    private GetMessageSourceCodeRequest(string id)
    {
        Id = id;
    }

    /// <summary>
    /// ID of the e-mail message of which the source code to get
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Create <see cref="GetMessageSourceCodeRequest"/>
    /// </summary>
    /// <param name="id">ID of the e-mail message of which the source code to get</param>
    /// <returns><see cref="GetMessageSourceCodeRequest"/></returns>
    public static GetMessageSourceCodeRequest Create(string id) => new(id);
}