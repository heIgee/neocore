﻿using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class Vendor : INeocoreNode<Vendor>
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? ContactInfo { get; set; }

    public static Vendor FromRecord(IRecord record)
    {
        var node = record[Al.Vendor].As<INode>();
        return node is null ? new() : FromNode(node);  
    }

    public static Vendor FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        ContactInfo = node.Properties["contactInfo"].As<string>()
    };
}
