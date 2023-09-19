using CountriesAPI.Contexts;
using CountriesAPI.Models;
using Dapper;
using System.Text;

namespace CountriesAPI.Repositories
{
    public class CountryTranslationRepository : ICountryTranslationRepository
    {
        private readonly IDapperContext _context;

        public CountryTranslationRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CountryTranslation>> GetCountryTranslationsAsync()
        {

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",CountryId");
            queryBuilder.Append(",LanguageCode");
            queryBuilder.Append(",Translation");
            queryBuilder.Append(" FROM CountryTranslations");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<CountryTranslation>(queryBuilder.ToString());
        }
    }
}
