using CountriesAPI.Models;

namespace CountriesAPI.Repositories
{
    public interface IStateTranslationRepository
    {
        Task<IEnumerable<StateTranslation>> GetStateTranslationsAsync();
    }
}
