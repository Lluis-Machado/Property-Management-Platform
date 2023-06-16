using AutoMapper;
using DocumentsAPI.Models;

namespace DocumentsAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FolderDTO, Folder>();
            CreateMap<Folder, FolderDTO>();
        }

    }
}
