﻿using Neo4j.Driver;

namespace Neocore.Models;

public class Item : INeocoreNode<Item>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Manufacturer { get; set; }
    public string? Specifications { get; set; }
    public float Price { get; set; }

    public static Item FromNode(IEntity node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        Type = node.Properties["type"].As<string>(),
        Manufacturer = node.Properties["manufacturer"].As<string>(),
        Specifications = node.Properties["specifications"].As<string>(),
        Price = node.Properties["price"].As<float>()
    };
}
