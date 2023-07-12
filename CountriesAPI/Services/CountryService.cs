using AutoMapper;
using CountriesAPI.DTOs;
using CountriesAPI.Models;
using CountriesAPI.Repositories;

namespace CountriesAPI.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICountryTranslationRepository _countryTranslationRepository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository countryRepository, ICountryTranslationRepository countryTranslationRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _countryTranslationRepository = countryTranslationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDTO>> GetCountriesAsync(string? languageCode = null)
        {
            if (languageCode is not null) return await GetTranslatedCountriesAsync(languageCode);

            return await GetAllCountriesAsync();
        }

        private async Task<IEnumerable<CountryDTO>> GetTranslatedCountriesAsync(string languageCode)
        {
            List<CountryDTO> countryDTOs = new();

            IEnumerable<Country> countries = await _countryRepository.GetCountriesAsync();

            IEnumerable<CountryTranslation> countryTranslations = await _countryTranslationRepository.GetCountryTranslationsAsync();

            foreach (Country country in countries)
            {
                CountryDTO countryDTO = _mapper.Map<CountryDTO>(country);

                if (countryTranslations.Any())
                {
                    CountryTranslation? countryTranslation = countryTranslations.FirstOrDefault(c => c.CountryId == country.Id && c.LanguageCode == languageCode);

                    if (countryTranslation is not null)
                    {
                        countryDTO.LanguageCode = languageCode;
                        countryDTO.Name = countryTranslation.Translation;
                    }
                }
                countryDTOs.Add(countryDTO);
            }
            return countryDTOs;
        }

        private async Task<IEnumerable<CountryDTO>> GetAllCountriesAsync()
        {
            IEnumerable<Country> countries = await _countryRepository.GetCountriesAsync();
            return _mapper.Map<IEnumerable<Country>, List<CountryDTO>>(countries);
        }
    }
}
