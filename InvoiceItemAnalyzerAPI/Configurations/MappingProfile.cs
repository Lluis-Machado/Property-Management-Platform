using AutoMapper;
using InvoiceItemAnalyzerAPI.DTOs;
using static InvoiceItemAnalyzerAPI.InvoiceItemModel;

namespace InvoiceItemAnalyzerAPI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ItemDTO, ModelInput>();
            CreateMap<ModelOutput, ItemCategoryPredictionDTO>()
              .ForMember(dest => dest.PredictedCategoryId, opt => opt.MapFrom(src => src.PredictedLabel))
              .ForMember(dest => dest.Confidence, opt => opt.MapFrom(src => src.Score[0]));
        }
    }
}
