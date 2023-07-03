using AutoMapper;
using PropertiesAPI.Models;

namespace PropertiesAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Property, PropertyDto>();
            CreateMap<Property, PropertyDetailedDto>();
            CreateMap<PropertyDto, Property>();
            CreateMap<PropertyDetailedDto, Property>();

            CreateMap<CreatePropertyDto, Property>();
            CreateMap<Property, CreatePropertyDto>();

            CreateMap<AddressDTO, PropertyAddress>();
            CreateMap<PropertyAddress, AddressDTO>();

        }

    }
}
