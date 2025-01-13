namespace TempMail.Client.Models;

/// <summary>
/// Domain info
/// </summary>
/// <param name="name">Domain name</param>
/// <param name="type">Domain type</param>
public class Domain(string name, DomainType type)
{
    /// <summary>
    /// Domain name
    /// </summary>
    public string Name { get; } = name;
    
    /// <summary>
    /// Domain type
    /// </summary>
    public DomainType Type { get; } = type;
}