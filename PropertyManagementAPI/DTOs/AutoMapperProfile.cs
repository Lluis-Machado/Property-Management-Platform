using AutoMapper;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Property, PropertyDTO>();
            CreateMap<Property, PropertyDetailedDTO>();
            CreateMap<PropertyDTO, Property>();
            CreateMap<PropertyDetailedDTO, Property>();

        }

    }
}
