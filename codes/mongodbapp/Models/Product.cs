using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mongodbapp.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}