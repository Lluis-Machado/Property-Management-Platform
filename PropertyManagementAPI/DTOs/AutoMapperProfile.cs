using AutoMapper;
using PropertiesAPI.Models;

namespace PropertiesAPI.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreatePropertyDto, Property>();
            CreateMap<Property, CreatePropertyDto>();

            CreateMap<UpdatePropertyDto, Property>();
            CreateMap<Property, UpdatePropertyDto>();

            CreateMap<AddressDto, PropertyAddress>();
            CreateMap<PropertyAddress, AddressDto>();

            CreateMap<Property, PropertyDetailedDto>();
            CreateMap<PropertyDetailedDto, Property>();

            CreateMap<Property, PropertyDto>();
            CreateMap<PropertyDto, Property>();


        }

    }
}
