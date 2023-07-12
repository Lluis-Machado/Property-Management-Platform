using CountriesAPI.Models;

namespace CountriesAPI.Repositories
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
    }
}
