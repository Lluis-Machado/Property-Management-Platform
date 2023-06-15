using System.Data;
using System.Data.SqlClient;

namespace AccountingAPI.Context
{
    public class DapperContext :IDapperContext
    {

        private readonly IDbConnection _connection;

        public DapperContext(string connectionString)
        {;
            _connection = new SqlConnection(connectionString);
        }

        public IDbConnection Connection => _connection;
    }
}
