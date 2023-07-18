using MongoDB.Driver;

namespace OwnershipAPI.Contexts;

public interface IMongoContext
{
    IMongoDatabase GetDatabase(string databaseName);
}