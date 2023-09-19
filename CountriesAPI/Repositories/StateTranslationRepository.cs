using CountriesAPI.Contexts;
using CountriesAPI.Models;
using Dapper;
using System.Text;

namespace CountriesAPI.Repositories
{
    public class StateTranslationRepository : IStateTranslationRepository
    {
        private readonly IDapperContext _context;

        public StateTranslationRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StateTranslation>> GetStateTranslationsAsync()
        {

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",StateId");
            queryBuilder.Append(",LanguageCode");
            queryBuilder.Append(",Translation");
            queryBuilder.Append(" FROM StateTranslations");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<StateTranslation>(queryBuilder.ToString());
        }
    }
}
