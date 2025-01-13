namespace TempMail.Client.Models;

public class Domain(string name, DomainType type)
{
    public string Name { get; } = name;
    public DomainType Type { get; } = type;
}