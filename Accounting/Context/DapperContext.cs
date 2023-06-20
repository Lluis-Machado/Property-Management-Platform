using System.Data;
using System.Data.SqlClient;

namespace AccountingAPI.Context
{
    public class DapperContext :IDapperContext
    {

        private readonly string _connectionString;


        public DapperContext(string connectionString)
        {;
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
