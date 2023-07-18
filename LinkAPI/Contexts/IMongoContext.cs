using MongoDB.Driver;

namespace LinkAPI.Contexts;

public interface IMongoContext
{
    IMongoDatabase GetDatabase(string databaseName);
}