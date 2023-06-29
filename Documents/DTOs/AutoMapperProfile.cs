using AutoMapper;
using DocumentsAPI.Models;

namespace DocumentsAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Folder, FolderDTO>();
            CreateMap<FolderDTO, Folder>();

            CreateMap<Folder, CreateFolderDTO>();
            CreateMap<CreateFolderDTO, Folder>();

            CreateMap<Folder, UpdateFolderDTO>();
            CreateMap<UpdateFolderDTO, Folder>();
        }

    }
}
