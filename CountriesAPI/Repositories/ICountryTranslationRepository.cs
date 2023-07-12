using CountriesAPI.Models;

namespace CountriesAPI.Repositories
{
    public interface ICountryTranslationRepository
    {
        Task<IEnumerable<CountryTranslation>> GetCountryTranslationsAsync();
    }
}
