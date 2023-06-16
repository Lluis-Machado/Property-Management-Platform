using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using OwnershipAPI.Models;
using ContactsAPI.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using ContactsAPI.Models;
using ContactsAPI.DTOs;
using PropertyManagementAPI.Models;

namespace ContactsAPI.Services
{
    public class ContactsService : IContactsService
    {
        private readonly IContactsRepository _contactsRepo;
        private readonly IValidator<ContactDTO> _contactValidator;
        private readonly IValidator<CreateContactDTO> _createContactValidator;
        private readonly IValidator<UpdateContactDTO> _updateContactValidator;
        private readonly IMapper _mapper;


        public ContactsService(IContactsRepository contactsRepo
            , IValidator<ContactDTO> contactValidator
            , IValidator<CreateContactDTO> createContactValidator
            , IValidator<UpdateContactDTO> updateContactValidator
            , IMapper mapper)
        {
            _contactsRepo = contactsRepo;
            _contactValidator = contactValidator;
            _createContactValidator = createContactValidator;
            _updateContactValidator = updateContactValidator;
            _mapper = mapper;
        }

        public async Task<ActionResult<ContactDTO>> CreateContactAsync(CreateContactDTO createContactDTO)
        {
            var contact = _mapper.Map<CreateContactDTO, Contact>(createContactDTO);

            contact = await _contactsRepo.InsertOneAsync(contact);

            var contactDTO = _mapper.Map<Contact, ContactDTO>(contact);
            return new CreatedResult($"contacts/{contactDTO.Id}", contactDTO);
        }

        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContactsAsync()
        {
            return new OkObjectResult(await _contactsRepo.GetAsync());
        }        

        public async Task<ContactDTO> GetContactByIdAsync(Guid id)
        {
            var contact = await _contactsRepo.GetContactByIdAsync(id);

            var contactDTO = _mapper.Map<Contact, ContactDTO>(contact);

            return contactDTO;
        } 
        public async Task<ContactDetailsDTO> GetContactWithProperties(Guid id)
        {
            var contact = await _contactsRepo.GetContactByIdAsync(id);

            var client = new OwnershipServiceClient();
            List<Ownership> ownership = await client.GetOwnershipByIdAsync(id);

            List<Property> properties = new List<Property>();

            var contactDTO = _mapper.Map<Contact, ContactDetailsDTO>(contact);


            foreach (Ownership item in ownership)
            {
                var clientP = new PropertyServiceClient();
                Property property = await clientP.GetPropertyByIdAsync(item.PropertyId);

                properties.Add(property);
                OwnershipInfoDTO ownershipInfo = new OwnershipInfoDTO()
                {
                    PropertyName = property.Name,
                    Share = item.Share
                };

                contactDTO.OwnershipInfo.Add(ownershipInfo);
            }

            return contactDTO;
        }
 
        public async Task<ActionResult<ContactDTO>> UpdateContactAsync(UpdateContactDTO updateContactDTO, Guid contactId)
        {
            var contact = _mapper.Map<UpdateContactDTO, Contact>(updateContactDTO);

            contact = await _contactsRepo.UpdateAsync(contact);

            var contactDTO = _mapper.Map<Contact, ContactDTO>(contact);

            return new OkObjectResult(updateContactDTO);
        }

        public async Task<IActionResult> DeleteContactAsync(Guid contactId)
        {
            var updateResult = await _contactsRepo.SetDeleteAsync(contactId, true);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteContactAsync(Guid contactId)
        {
            var updateResult = await _contactsRepo.SetDeleteAsync(contactId, false);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Contact not found");

            return new NoContentResult();
        }
    }
}
