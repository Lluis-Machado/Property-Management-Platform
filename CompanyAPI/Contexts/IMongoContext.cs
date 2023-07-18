using MongoDB.Driver;

namespace CompanyAPI.Contexts
{
    public interface IMongoContext
    {
        IMongoDatabase GetDataBase(string databaseName);
    }
}