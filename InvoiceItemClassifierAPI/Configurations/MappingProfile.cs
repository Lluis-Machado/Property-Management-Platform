using AutoMapper;
using InvoiceItemClassifierAPI.DTOs;
using static InvoiceItemClassifierAPI.InvoiceLine;

namespace InvoiceItemClassifierAPI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InvoiceItemDTO, ModelInput>();
            CreateMap<ModelOutput, InvoiceItemCategoryPredictionDTO>()
              .ForMember(dest => dest.PredictedCategoryId, opt => opt.MapFrom(src => src.PredictedLabel))
              .ForMember(dest => dest.Confidence, opt => opt.MapFrom(src => src.Score[0]));
        }
    }
}
