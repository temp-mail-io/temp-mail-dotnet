namespace TempMail.Client.Requests;

/// <summary>
/// Request to delete specific e-mail message
/// </summary>
public class DeleteSpecificMessageRequest
{
    private DeleteSpecificMessageRequest(string id)
    {
        Id = id;
    }

    /// <summary>
    /// ID of the message to delete
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Create <see cref="DeleteSpecificMessageRequest"/>
    /// </summary>
    /// <param name="id">ID of the message to delete</param>
    /// <returns><see cref="DeleteSpecificMessageRequest"/></returns>
    public static DeleteSpecificMessageRequest Create(string id) => new(id);
}