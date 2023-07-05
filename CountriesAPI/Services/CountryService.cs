using AutoMapper;
using CountriesAPI.DTOs;
using CountriesAPI.Models;
using CountriesAPI.Repositories;

namespace CountriesAPI.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDTO>> GetCountriesAsync()
        {
            IEnumerable<Country> countries = await _countryRepository.GetCountriesAsync();

            return _mapper.Map<IEnumerable<Country>, List<CountryDTO>>(countries);
        }
    }
}
