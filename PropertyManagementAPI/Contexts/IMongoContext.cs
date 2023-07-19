using MongoDB.Driver;

namespace PropertiesAPI.Contexts
{
    public interface IMongoContext
    {
        IMongoDatabase GetDataBase(string databaseName);
    }
}