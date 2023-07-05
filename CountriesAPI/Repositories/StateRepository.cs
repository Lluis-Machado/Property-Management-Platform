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

        public async Task<IEnumerable<State>> GetStatesAsync(string? countryCode = null)
        {
            var parameters = new
            {
                countryCode
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT StateCode");
            queryBuilder.Append(",CountryCode");
            queryBuilder.Append(",Name");
            queryBuilder.Append(" FROM States");
            if(countryCode is not null) queryBuilder.Append(" WHERE CountryCode = @countryCode");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<State>(queryBuilder.ToString(), parameters);
        }
    }
}
