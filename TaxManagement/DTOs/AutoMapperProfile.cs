using AutoMapper;
using TaxManagement.Models;

namespace TaxManagementAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DeclarantDTO, Declarant>();
            CreateMap<CreateDeclarantDTO, Declarant>();
            CreateMap<UpdateDeclarantDTO, Declarant>();
            CreateMap<DeclarantDetailedDTO, Declarant>();
            CreateMap<Declarant, DeclarantDetailedDTO>();
            CreateMap<Declarant, DeclarantDTO>();
            CreateMap<Declarant, CreateDeclarantDTO>();
            CreateMap<Declarant, UpdateDeclarantDTO>();

            CreateMap<DeclarationDTO, Declaration>();
            CreateMap<CreateDeclarationDTO, Declaration>();
            CreateMap<UpdateDeclarationDTO, Declaration>();
            CreateMap<DeclarationDetailedDTO, Declaration>();
            CreateMap<Declaration, DeclarationDetailedDTO>();
            CreateMap<Declaration, DeclarationDTO>();
            CreateMap<Declaration, CreateDeclarationDTO>();
            CreateMap<Declaration, UpdateDeclarationDTO>();

        }

    }
}
