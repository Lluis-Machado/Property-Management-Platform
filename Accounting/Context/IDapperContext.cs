using System.Data;

namespace AccountingAPI.Context
{
    public interface IDapperContext
    {
        IDbConnection Connection { get; }
    }
}
