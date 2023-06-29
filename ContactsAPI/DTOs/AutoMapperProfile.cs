using AutoMapper;
using ContactsAPI.Models;

namespace ContactsAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Contact, ContactDTO>();
            CreateMap<Contact, CreateContactDTO>();
            CreateMap<Contact, UpdateContactDTO>();
            CreateMap<Contact, ContactDetailsDTO>();
            CreateMap<ContactDetailsDTO, Contact>();
            CreateMap<ContactDTO, Contact>();
            CreateMap<CreateContactDTO, Contact>();
            CreateMap<UpdateContactDTO, Contact>();

        }

    }
}
