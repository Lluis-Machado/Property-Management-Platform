using System.Data;

namespace CountriesAPI.Contexts
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
