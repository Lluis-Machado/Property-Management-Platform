using AutoMapper;
using OwnershipAPI.Models;

namespace OwnershipAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Ownership, OwnershipDTO>();
            CreateMap<OwnershipDTO, Ownership>();

        }

    }
}
