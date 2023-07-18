using AutoMapper;
using ContactsAPI.DTOs;
using ContactsAPI.Models;
using ContactsAPI.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Services
{
    public class ContactsService : IContactsService
    {
        private readonly IContactsRepository _contactsRepo;
        private readonly IValidator<ContactDTO> _contactValidator;
        private readonly IValidator<CreateContactDto> _createContactValidator;
        private readonly IValidator<UpdateContactDTO> _updateContactValidator;
        private readonly IMapper _mapper;


        public ContactsService(IContactsRepository contactsRepo
            , IValidator<ContactDTO> contactValidator
            , IValidator<CreateContactDto> createContactValidator
            , IValidator<UpdateContactDTO> updateContactValidator
            , IMapper mapper)
        {
            _contactsRepo = contactsRepo;
            _contactValidator = contactValidator;
            _createContactValidator = createContactValidator;
            _updateContactValidator = updateContactValidator;
            _mapper = mapper;
        }

        public async Task<ActionResult<ContactDetailedDto>> CreateAsync(CreateContactDto createContactDto, string lastUser)
        {
            var contact = _mapper.Map<CreateContactDto, Contact>(createContactDto);

            contact.LastUpdateByUser = lastUser;
            contact.CreatedByUser = lastUser;
            contact.CreatedAt = DateTime.UtcNow;
            contact.LastUpdateAt = DateTime.UtcNow;

            contact = await _contactsRepo.InsertOneAsync(contact);

            var contactDto = _mapper.Map<Contact, ContactDetailedDto>(contact);
            return new CreatedResult($"contacts/{contactDto.Id}", contactDto);
        }

        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetAsync(bool includeDeleted = false)
        {
            var contacts = await _contactsRepo.GetAsync(includeDeleted);

            return new OkObjectResult(contacts);
        }

        public async Task<ContactDetailedDto> GetByIdAsync(Guid id)
        {
            var contact = await _contactsRepo.GetContactByIdAsync(id);

            var contactDto = _mapper.Map<Contact, ContactDetailedDto>(contact);

            return contactDto;
        }
        public async Task<ContactDetailsDTO> GetWithProperties(Guid id)
        {
            var contact = await _contactsRepo.GetContactByIdAsync(id);

            var client = new OwnershipServiceClient();
            List<ContactOwnershipDTO> ownerships = await client.GetOwnershipByIdAsync(id) ?? new List<ContactOwnershipDTO>();

            var contactDTO = _mapper.Map<Contact, ContactDetailsDTO>(contact);

            if (contactDTO.OwnershipInfo is null)
                contactDTO.OwnershipInfo = new List<ContactOwnershipInfoDTO>();

            if (ownerships is not null)
            {
                foreach (var item in ownerships)
                {
                    var clientP = new PropertyServiceClient();
                    var property = await clientP.GetPropertyByIdAsync(item.PropertyId) ?? null;
                    if (property is not null)
                    {
                        ContactOwnershipInfoDTO ownershipInfo = new ContactOwnershipInfoDTO()
                        {
                            PropertyName = property.Name ?? "",
                            Share = item.Share,
                            PropertyId = item.PropertyId
                        };
                        contactDTO.OwnershipInfo.Add(ownershipInfo);
                    }
                }
            }

            return contactDTO;
        }

        public async Task<ActionResult<ContactDetailedDto>> UpdateContactAsync(Guid contactId, UpdateContactDTO updateContactDTO, string lastUser)
        {
            var contact = _mapper.Map<UpdateContactDTO, Contact>(updateContactDTO);

            contact.LastUpdateByUser = lastUser;
            contact.LastUpdateAt = DateTime.UtcNow;
            contact.Id = contactId;

            contact = await _contactsRepo.UpdateAsync(contact);

            var contactDTO = _mapper.Map<Contact, ContactDetailedDto>(contact);

            return new OkObjectResult(contactDTO);
        }

        public async Task<bool> CheckIfNIEUnique(string NIE, Guid? contactId)
        {
            var exist = await _contactsRepo.CheckIfNIEUnique(NIE, contactId);

            return exist;
        }

        public async Task<IActionResult> DeleteContactAsync(Guid contactId, string lastUser)
        {
            var updateResult = await _contactsRepo.SetDeleteAsync(contactId, true, lastUser);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteContactAsync(Guid contactId, string lastUser)
        {
            var updateResult = await _contactsRepo.SetDeleteAsync(contactId, false, lastUser);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

            return new NoContentResult();
        }

        public async Task<IEnumerable<ContactDTO>> GetPaginatedContactsAsync(int pageNumber, int pageSize)
        {
            // Calculate the number of items to skip based on the page number and size
            int itemsToSkip = (pageNumber - 1) * pageSize;

            // Retrieve the declarants based on the pagination parameters
            IEnumerable<Contact> contacts = await _contactsRepo.GetAsync();

            // Apply pagination by skipping the required number of items and taking only the specified page size
            IEnumerable<Contact> paginatedContacts = contacts.Skip(itemsToSkip).Take(pageSize);
            IEnumerable<ContactDTO> paginatedContactsDTO = _mapper.Map<IEnumerable<Contact>, IEnumerable<ContactDTO>>(paginatedContacts);

            return paginatedContactsDTO;
        }
    }
}
