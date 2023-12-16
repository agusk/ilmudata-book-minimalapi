using MongoDB.Driver;

namespace mongodbapp.Models;
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("MongoDbDemo");
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
}