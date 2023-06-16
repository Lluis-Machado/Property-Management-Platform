using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.DTOs;
using PropertyManagementAPI.Models;
using PropertyManagementAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Services
{
    public class PropertiesService : IPropertiesService
    {
        private readonly IPropertiesRepository _propertiesRepo;
        private readonly IMapper _mapper;

        public PropertiesService(IPropertiesRepository propertiesRepo, IMapper mapper)
        {
            _propertiesRepo = propertiesRepo;
            _mapper = mapper;

        }

        public async Task<ActionResult<PropertyDTO>> CreateProperty(CreatePropertyDTO createPropertyDTO, string userName)
        {
            var property = _mapper.Map<CreatePropertyDTO, Property>(createPropertyDTO);
            property.CreatedByUser = userName;
            property.LastUpdateByUser = userName;
            property.LastUpdateAt = DateTime.UtcNow;
            property.CreatedAt = DateTime.UtcNow;

            property = await _propertiesRepo.InsertOneAsync(property);

            var propertyDTO = _mapper.Map<Property, PropertyDTO>(property);

            return new CreatedResult($"properties/{propertyDTO.Id}", propertyDTO);
        }

        public async Task<ActionResult<IEnumerable<PropertyDTO>>> GetProperties()
        {
            var result = await _propertiesRepo.GetAsync();
            IEnumerable<PropertyDTO> propertyDTOs = _mapper.Map<IEnumerable<PropertyDTO>>(result);

            return new OkObjectResult(propertyDTOs);
        }

        public async Task<ActionResult<PropertyDTO>> UpdateProperty(Guid propertyId, UpdatePropertyDTO updatePropertyDTO, string lastUser)
        {
            var property = _mapper.Map<UpdatePropertyDTO, Property>(updatePropertyDTO);

            property.LastUpdateAt = DateTime.UtcNow;
            property.LastUpdateByUser = lastUser;

            property = await _propertiesRepo.UpdateAsync(property);

            var propertyDTO = _mapper.Map<Property, PropertyDTO>(property);

            return new OkObjectResult(propertyDTO);
        }

        public async Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser)
        {
            await _propertiesRepo.SetDeleteDeclarantAsync(propertyId, true, lastUser);

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteProperty(Guid propertyId, string lastUser)
        {
            await _propertiesRepo.SetDeleteDeclarantAsync(propertyId, false, lastUser);

            return new NoContentResult();
        }

        public async Task<bool> PropertyExists(Guid propertyId)
        {
            Property? property = await _propertiesRepo.GetPropertyByIdAsync(propertyId);
            return (property != null);
        } 
        public async Task<ActionResult<PropertyDTO>> GetProperty(Guid propertyId)
        {

            var property = await _propertiesRepo.GetPropertyByIdAsync(propertyId);

            var propertyDTO = _mapper.Map<Property, PropertyDTO>(property);

            return new OkObjectResult(propertyDTO);
        }
    }
}
