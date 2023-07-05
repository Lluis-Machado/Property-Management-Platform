using AutoMapper;
using CountriesAPI.DTOs;
using CountriesAPI.Models;

namespace CountriesAPI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Country
            CreateMap<Country, CountryDTO>();
            // State
            CreateMap<State, StateDTO>();
        }
    }
}
