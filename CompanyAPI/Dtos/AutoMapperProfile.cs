using AutoMapper;
using CompanyAPI.Models;

namespace CompanyAPI.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddressDto, CompanyAddress>().ReverseMap();
            CreateMap<CompanyDto, Company>().ReverseMap();
            CreateMap<CompanyDetailedDto, Company>().ReverseMap();
            CreateMap<CreateCompanyDto, Company>().ReverseMap();
            CreateMap<UpdateCompanyDto, Company>().ReverseMap();

          
        }

    }
}
