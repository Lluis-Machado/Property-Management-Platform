using MongoDB.Driver;

namespace ContactsAPI.Contexts
{
    public interface IMongoContext
    {
        IMongoDatabase GetDataBase(string databaseName);
    }
}