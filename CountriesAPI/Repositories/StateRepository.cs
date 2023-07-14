using CountriesAPI.Contexts;
using CountriesAPI.Models;
using System.Text;
using Dapper;

namespace CountriesAPI.Repositories
{
    public class StateRepository :IStateRepository
    {
        private readonly IDapperContext _context;

        public StateRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<State>> GetStatesAsync(int? countryId = null)
        {
            var parameters = new
            {
                countryId
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",CountryId");
            queryBuilder.Append(",StateCode");
            queryBuilder.Append(",Name");
            queryBuilder.Append(" FROM States");
            if(countryId is not null) queryBuilder.Append(" WHERE CountryId = @countryId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<State>(queryBuilder.ToString(), parameters);
        }
    }
}
