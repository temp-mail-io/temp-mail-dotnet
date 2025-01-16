namespace TempMail.Client.Models;

/// <summary>
/// Attachment to mail message
/// </summary>
/// <param name="id">ID of the attachment</param>
/// <param name="name">Attachment name</param>
/// <param name="size">Attachment size</param>
public class Attachment(
    string id,
    string name,
    int size)
{
    /// <summary>
    /// Attachment ID
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Attachment name
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Attachment size
    /// </summary>
    public int Size { get; } = size;
}