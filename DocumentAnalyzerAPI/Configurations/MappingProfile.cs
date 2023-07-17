using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using AutoMapper;

namespace DocumentAnalyzerAPI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BoundingRegion, BoundingRegionDTO>();
        }
    }
}
