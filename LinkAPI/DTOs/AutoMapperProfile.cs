using AutoMapper;
using LinkAPI.Models;

namespace LinkAPI.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Link, LinkDto>();
            CreateMap<LinkDto, Link>();

        }

    }
}
