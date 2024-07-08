using Neo4j.Driver;

namespace Neocore.Models;

public class Vendor : INeocoreNode<Vendor>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ContactInfo { get; set; }

    //public static Vendor FromRecord(IEntity entity) => FromNode(entity.As<INode>());

    public static Vendor FromNode(IEntity node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        ContactInfo = node.Properties["contactInfo"].As<string>()
    };
}
