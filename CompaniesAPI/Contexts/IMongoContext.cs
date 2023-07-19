using MongoDB.Driver;

namespace CompaniesAPI.Contexts
{
    public interface IMongoContext
    {
        IMongoDatabase GetDataBase(string databaseName);
    }
}