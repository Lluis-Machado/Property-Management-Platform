using AutoMapper;
using ContactsAPI.Models;

namespace ContactsAPI.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateContactDTO, Contact>();
            CreateMap<Contact, CreateContactDTO>();

            CreateMap<ContactAddressDTO, ContactAddress>();
            CreateMap<ContactAddress, ContactAddressDTO>();

            CreateMap<CreateContactDTO, ContactData>();
            CreateMap<ContactData, CreateContactDTO>();

            CreateMap<Contact, ContactDetailedDTO>();
            CreateMap<ContactDetailedDTO, Contact>();

            CreateMap<ContactDetailedDTO, ContactData>();
            CreateMap<ContactData, ContactDetailedDTO>();

            CreateMap<Contact, ContactDTO>();
            CreateMap<Contact, ContactDetailsDTO>();
            CreateMap<ContactDetailsDTO, Contact>();
            CreateMap<ContactDTO, Contact>();
            
            CreateMap<UpdateContactDTO, Contact>();
            CreateMap<Contact, UpdateContactDTO>();






        }

    }
}
