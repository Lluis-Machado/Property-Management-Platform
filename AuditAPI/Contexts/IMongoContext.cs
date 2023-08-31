using MongoDB.Driver;

namespace AuditsAPI.Contexts
{
    public interface IMongoContext
    {
        IMongoDatabase GetDataBase(string databaseName);
    }
}