using CountriesAPI.DTOs;

namespace CountriesAPI.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDTO>> GetCountriesAsync();
    }
}
