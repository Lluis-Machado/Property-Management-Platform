using AutoMapper;
using ContactsAPI.Models;

namespace ContactsAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateContactDto, Contact>();
            CreateMap<Contact, CreateContactDto>();

            CreateMap<AddressDto, ContactAddress>();
            CreateMap<ContactAddress, AddressDto>();

            CreateMap<Contact, ContactDetailedDto>();
            CreateMap<ContactDetailedDto, Contact>();

            CreateMap<Contact, ContactDTO>();
            CreateMap<Contact, ContactDetailsDTO>();
            CreateMap<ContactDetailsDTO, Contact>();
            CreateMap<ContactDTO, Contact>();
            
            CreateMap<UpdateContactDTO, Contact>();
            CreateMap<Contact, UpdateContactDTO>();






        }

    }
}