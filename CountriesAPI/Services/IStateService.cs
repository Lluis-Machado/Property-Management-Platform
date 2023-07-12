using CountriesAPI.DTOs;

namespace CountriesAPI.Services
{
    public interface IStateService
    {
        Task<IEnumerable<StateDTO>> GetStatesAsync(string? languageCode = null);
        Task<IEnumerable<StateDTO>> GetStatesByCountryAsync(int countryId, string? languageCode = null);
    }
}
