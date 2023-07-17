using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PropertiesAPI.Services;
using PropertiesAPI.Dtos;
using PropertiesAPI.Exceptions;
using PropertiesAPI.Models;
using PropertiesAPI.Repositories;

namespace PropertiesAPI.Services
{
    public class PropertiesService : IPropertiesService
    {
        private readonly IPropertiesRepository _propertiesRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePropertyDto> _createPropertyValidator;
        private readonly IValidator<UpdatePropertyDto> _updatePropertyValidator;

        public PropertiesService(IPropertiesRepository propertiesRepo,
            IMapper mapper,
            IValidator<CreatePropertyDto> createPropertyValidator,
            IValidator<UpdatePropertyDto> updatePropertyValidator)
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
            if(createPropertyDto.Address is not null)
                property.PropertyAddress = _mapper.Map<AddressDto, PropertyAddress>(createPropertyDto.Address);

            property.CreatedByUser = userName;
            property.LastUpdateByUser = userName;
            property.LastUpdateAt = DateTime.UtcNow;
            property.CreatedAt = DateTime.UtcNow;

            property = await _propertiesRepo.InsertOneAsync(property);
            
            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);
            
            propertyDto.Address = _mapper.Map<PropertyAddress, AddressDto>(property.PropertyAddress);
            
            if(property.MainOwnerType == "Contact")
                propertyDto.MainOwner = await GetContactData(property.MainOwnerId);
            //else if (property.MainOwnerType == "Company")
            //    propertyDto.MainOwner = await GetCompanyData(property.MainOwnerId);

            return new CreatedResult($"properties/{propertyDto.Id}", propertyDto);
        }

      

        public async Task<ActionResult<PropertyDetailedDto>> UpdateProperty(UpdatePropertyDto updatePropertyDto, string lastUser, Guid propertyId)
        {
            // validation
            await _updatePropertyValidator.ValidateAndThrowAsync(updatePropertyDto);

            await PropertyExists(propertyId);

            var property = _mapper.Map<UpdatePropertyDto, Property>(updatePropertyDto);

            if (updatePropertyDto.Address is not null)
                property.PropertyAddress = _mapper.Map<AddressDto, PropertyAddress>(updatePropertyDto.Address);
            
            property.LastUpdateAt = DateTime.UtcNow;
            property.LastUpdateByUser = lastUser;
            property.Id = propertyId;
            
            property = await _propertiesRepo.UpdateAsync(property);
            
            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);
            propertyDto.Address = updatePropertyDto.Address!;

            if (property.MainOwnerType == "Contact")
                propertyDto.MainOwner = await GetContactData(property.MainOwnerId);
            //else if (property.MainOwnerType == "Company")
            //    propertyDto.MainOwner = await GetCompanyData(property.MainOwnerId);

            if (property.ParentPropertyId is not null)
                propertyDto.ParentProperty = await GetParentPropertyData((Guid)property.ParentPropertyId);

            return new OkObjectResult(propertyDto);
        }

        private async Task<BasicPropertyDto> GetParentPropertyData(Guid propertyParentPropertyId)
        {
            var result = await _propertiesRepo.GetByIdAsync(propertyParentPropertyId);

            BasicPropertyDto property = new();
            property.Id = propertyParentPropertyId;
            property.PropertyName = result.Name!;
            return property;
        }


        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(bool includeDeleted = false)
        {
            var results = await _propertiesRepo.GetAsync();
            var propertyDtOs = results.Any() ? _mapper.Map<IEnumerable<PropertyDto>>(results) : Enumerable.Empty<PropertyDto>();
            

            foreach (var property in propertyDtOs)
            {
                foreach (var result in results)
                {
                    if (result.Id != property.Id) continue;
                    var address = _mapper.Map<PropertyAddress, AddressDto>(result.PropertyAddress);
                    property.Address = address;

                    if (result.MainOwnerType == "Contact")
                            property.MainOwner = await GetContactData(result.MainOwnerId);
                    //else if (property.MainOwnerType == "Company")
                    //    propertyDto.MainOwner = await GetCompanyData(property.MainOwnerId);

                    if (result.ParentPropertyId is not null)
                        property.ParentProperty = await GetParentPropertyData((Guid)result.ParentPropertyId);
                }
            }
            
            return new OkObjectResult(propertyDtOs);
        }

        public async Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser)
        {
            await PropertyExists(propertyId);

            await _propertiesRepo.SetDeleteAsync(propertyId, true, lastUser);

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteProperty(Guid propertyId, string lastUser)
        {
            await PropertyExists(propertyId);

            await _propertiesRepo.SetDeleteAsync(propertyId, false, lastUser);

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
            propertyDto.Address = _mapper.Map<PropertyAddress, AddressDto>(property.PropertyAddress);
            
            //var clientO = new OwnershipServiceClient();
            //var ownerships = await clientO.GetOwnershipByIdAsync(propertyId);
            


            if (property.ParentPropertyId is not null)
                propertyDto.ParentProperty = await GetParentData((Guid)property.ParentPropertyId);

            if(property.MainOwnerType == "Contact")
                propertyDto.MainOwner = await GetContactData(property.MainOwnerId);

            return propertyDto;
        }
        
        private async Task<OwnerDto> GetContactData(Guid id)
        {
            var contactDto = new OwnerDto();

            var clientC = new ContactServiceClient();
            var contact = await clientC.GetContactByIdAsync(id);

            contactDto.Id = contact!.Id;
            contactDto.OwnerName  = contact.FirstName + " " + contact.LastName;
            contactDto.OwnerType = "Contact";

            return contactDto;
        }

        private async Task<BasicPropertyDto> GetParentData(Guid id)
        {
            var propertyDto = new BasicPropertyDto();
            var property = await _propertiesRepo.GetByIdAsync(id);
            propertyDto.Id = property.Id;
            propertyDto.PropertyName = property.Name!;

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
                propertyDto.PropertyName = property.Name!;
                childProperties.Add(propertyDto);
            }

            return childProperties;
        }
    }
}
