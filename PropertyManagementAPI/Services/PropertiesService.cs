using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using PropertiesAPI.DTOs;
using PropertiesAPI.Models;
using PropertiesAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PropertiesAPI.Exceptions;

namespace PropertiesAPI.Services
{
    public class PropertiesService : IPropertiesService
    {
        private readonly IPropertiesRepository _propertiesRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePropertyDto> _createPropertyValidator;
        private readonly IValidator<UpdatePropertyDTO> _updatePropertyValidator;

        public PropertiesService(IPropertiesRepository propertiesRepo,
            IMapper mapper,
            IValidator<CreatePropertyDto> createPropertyValidator,
            IValidator<UpdatePropertyDTO> updatePropertyValidator)
        {
            _propertiesRepo = propertiesRepo;
            _mapper = mapper;
            _createPropertyValidator = createPropertyValidator;
            _updatePropertyValidator = updatePropertyValidator;
            _updatePropertyValidator = updatePropertyValidator;
        }

        public async Task<ActionResult<PropertyDetailedDto>> CreateProperty(CreatePropertyDto createPropertyDto, string userName)
        {
            // validation
            await _createPropertyValidator.ValidateAndThrowAsync(createPropertyDto);

            var property = _mapper.Map<CreatePropertyDto, Property>(createPropertyDto);
            property.CreatedByUser = userName;
            property.LastUpdateByUser = userName;
            property.LastUpdateAt = DateTime.UtcNow;
            property.CreatedAt = DateTime.UtcNow;

            var ownerships = createPropertyDto.Ownerships;
            var childProperties = createPropertyDto.ChildProperties;

            property.MainContactId = ownerships.FirstOrDefault(o => o.MainOwnership)!.ContactId;

            property = await _propertiesRepo.InsertOneAsync(property);
            
            if (childProperties.Any())
            {
                foreach (var childProperty in childProperties)
                {
                    await _propertiesRepo.UpdateParentIdAsync(property.Id, (Guid)childProperty!);
                }
            }
            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);

            foreach (var item in ownerships)
            {
                item.PropertyId = property.Id;
                var clientO = new OwnershipServiceClient();
                var ownership = await clientO.CreateOwnershipAsync(item);
                if (ownership is not null)
                {
                    propertyDto.Ownerships.Add(ownership);
                }                
            }

            if (property.ParentPropertyId is not null)
                propertyDto.ParentProperty = await GetParentData((Guid)property.ParentPropertyId);

            if (propertyDto.ChildProperties.Any())
                propertyDto.ChildProperties = await GetChildData(propertyDto.Id);

            propertyDto.MainContact = await GetContactData(property.MainContactId);
            
            return new CreatedResult($"properties/{propertyDto.Id}", propertyDto);
        }

      

        public async Task<ActionResult<PropertyDetailedDto>> UpdateProperty(UpdatePropertyDTO updatePropertyDto, string lastUser, Guid propertyId)
        {
            // validation
            await _updatePropertyValidator.ValidateAndThrowAsync(updatePropertyDto);

            // check if exists
            await PropertyExists(propertyId);

            var property = _mapper.Map<UpdatePropertyDTO, Property>(updatePropertyDto);

            property.LastUpdateAt = DateTime.UtcNow;
            property.LastUpdateByUser = lastUser;
            property.Id = propertyId;

            var ownerships = updatePropertyDto.Ownerships;
            var childProperties = updatePropertyDto.ChildProperties;

            property.MainContactId = ownerships.FirstOrDefault(o => o.MainOwnership)!.ContactId;

            property = await _propertiesRepo.UpdateAsync(property);

            if (childProperties.Any())
            {
                foreach (var childProperty in childProperties)
                {
                    await _propertiesRepo.UpdateParentIdAsync(property.Id, (Guid)childProperty!);
                }
            }

            //var ownerships = updatePropertyDto.Ownerships;

            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);
            var clientO = new OwnershipServiceClient();


            foreach (var item in ownerships)
            {
                var ownership = await clientO.UpsertOwnershipAsync(item);
                if (ownership is not null)
                {
                    propertyDto.Ownerships.Add(ownership);
                }
            }

            if (property.ParentPropertyId is not null)
                propertyDto.ParentProperty = await GetParentData((Guid)property.ParentPropertyId);

            if (propertyDto.ChildProperties.Any())
                propertyDto.ChildProperties = await GetChildData(propertyDto.Id);

            propertyDto.MainContact = await GetContactData(property.MainContactId);

            return new OkObjectResult(propertyDto);
        }


        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(bool includeDeleted = false)
        {
            var result = await _propertiesRepo.GetAsync();
            var propertyDtOs = result.Any() ? _mapper.Map<IEnumerable<PropertyDetailedDto>>(result) : Enumerable.Empty<PropertyDetailedDto>();

            

            foreach (var property in propertyDtOs)
            {
                var clientO = new OwnershipServiceClient();
                var ownerships = await clientO.GetOwnershipByIdAsync(property.Id);
                if (ownerships is not null)
                    foreach(var ownership in ownerships)
                    {
                        if(ownership.MainOwnership)
                        {
                            var client = new ContactServiceClient();
                            ContactDto? contact = await client.GetContactByIdAsync(ownership.ContactId);
                            if (contact is not null)
                            {
                                property.MainContact.FirstName = contact.FirstName;
                                property.MainContact.LastName = contact.LastName;
                                property.MainContact.Id = contact.Id;

                            }
                        }
                    }
            }

            return new OkObjectResult(propertyDtOs);
        }

        public async Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser)
        {
            await PropertyExists(propertyId);

            await _propertiesRepo.SetDeleteDeclarantAsync(propertyId, true, lastUser);

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteProperty(Guid propertyId, string lastUser)
        {
            await PropertyExists(propertyId);

            await _propertiesRepo.SetDeleteDeclarantAsync(propertyId, false, lastUser);

            return new NoContentResult();
        }

        public async Task<bool> PropertyExists(Guid propertyId)
        {
            Property? property = await _propertiesRepo.GetPropertyByIdAsync(propertyId);
            if (property is null) throw new NotFoundException("Property");

            return true;
        } 
        public async Task<ActionResult<PropertyDetailedDto>> GetProperty(Guid propertyId)
        {
           return new OkObjectResult(await GetPropertyDto(propertyId));
        }

        private async Task<PropertyDetailedDto> GetPropertyDto(Guid propertyId)
        {
            var property = await _propertiesRepo.GetPropertyByIdAsync(propertyId);

            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);

            var clientO = new OwnershipServiceClient();
            var ownerships = await clientO.GetOwnershipByIdAsync(propertyId);

            propertyDto.Ownerships = ownerships!;

            foreach (var propertyDtoOwnership in propertyDto.Ownerships)
            {
                propertyDtoOwnership.ContactDetail = await GetContactData(propertyDtoOwnership.ContactId);
            } 



            if (property.ParentPropertyId is not null)
                propertyDto.ParentProperty = await GetParentData((Guid)property.ParentPropertyId);

            if (propertyDto.ChildProperties.Any())
                propertyDto.ChildProperties = await GetChildData(propertyDto.Id);

            propertyDto.MainContact = await GetContactData(property.MainContactId);

            return propertyDto;
        }

        public class OwnershipComparer : IEqualityComparer<PropertyOwnershipDto>
        {
            public bool Equals(PropertyOwnershipDto? x, PropertyOwnershipDto? y)
            {
                return x!.Id == y!.Id;
            }

            public int GetHashCode(PropertyOwnershipDto obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        private async Task<ContactDto> GetContactData(Guid id)
        {
            var contactDto = new ContactDto();

            var clientC = new ContactServiceClient();
            var contact = await clientC.GetContactByIdAsync(id);

            contactDto.Id = contact!.Id;
            contactDto.FirstName = contact.FirstName;
            contactDto.LastName = contact.LastName;

            return contactDto;
        }

        private async Task<BasicPropertyDto> GetParentData(Guid id)
        {
            var propertyDto = new BasicPropertyDto();
            var property = await _propertiesRepo.GetByIdAsync(id);
            propertyDto.Id = property.Id;
            propertyDto.Name = property.Name!;

            return propertyDto;
        }

        private async Task<List<BasicPropertyDto>> GetChildData(Guid id)
        {
            var childProperties = new List<BasicPropertyDto>();

            var properties = await _propertiesRepo.GetPropertiesByParentIdAsync(id);

            foreach (var property in properties)
            {
                var propertyDto = new BasicPropertyDto();
                propertyDto.Id = property.Id;
                propertyDto.Name = property.Name!;
                childProperties.Add(propertyDto);
            }

            return childProperties;
        }
    }
}
