using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class User : INeocoreNode<User>
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public UserRole? Role { get; set; }
    public string? Password { get; set; }

    public static User FromRecord(IRecord record)
    {
        var node = record[Al.User].As<INode>();
        return node is null ? new() : FromNode(node); 
    }

    public static User FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        Role = Enum.Parse<UserRole>(
            node.Properties["role"].As<string>(), 
            ignoreCase: true
        ),
        Password = node.Properties["password"].As<string>(),
    };
}