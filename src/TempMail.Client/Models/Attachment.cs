namespace TempMail.Client.Models;

public class Attachment(
    string id,
    string name,
    int size)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public int Size { get; } = size;
}