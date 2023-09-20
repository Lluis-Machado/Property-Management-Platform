using AutoMapper;
using FluentValidation;
using MassTransit;
using MessagingContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PropertiesAPI.Dtos;
using PropertiesAPI.Exceptions;
using PropertiesAPI.Models;
using PropertiesAPI.Repositories;
using System.Text.Json;
using System.Xml;

namespace PropertiesAPI.Services
{
    public class PropertiesService : IPropertiesService
    {
        private readonly IPropertiesRepository _propertiesRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePropertyDto> _createPropertyValidator;
        private readonly IValidator<UpdatePropertyDto> _updatePropertyValidator;
        private readonly IPublishEndpoint _publishEndpoint;

        public PropertiesService(IPropertiesRepository propertiesRepo,
            IMapper mapper,
            IValidator<CreatePropertyDto> createPropertyValidator,
            IValidator<UpdatePropertyDto> updatePropertyValidator,
            IPublishEndpoint publishEndpoint
            )
        {
            _propertiesRepo = propertiesRepo;
            _mapper = mapper;
            _createPropertyValidator = createPropertyValidator;
            _updatePropertyValidator = updatePropertyValidator;
            _publishEndpoint = publishEndpoint;

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

            if (!string.IsNullOrEmpty(property.CadastreRef))
            {
                string catUrl = await CreateCatastreURL(property.CadastreRef);
                if (catUrl == "invalid")
                    return new BadRequestObjectResult("Invalid cadastre ref.");
                property.CadastreUrl = catUrl;
            }

            property = await _propertiesRepo.InsertOneAsync(property);

            _ = PerformAudit(property, property.Id);

            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);

            if (!createPropertyDto.MainOwnerType.IsNullOrEmpty())
                await CreateMainOwnership(createPropertyDto.MainOwnerId, createPropertyDto.MainOwnerType, property.Id);

            return new CreatedResult($"properties/{propertyDto.Id}", propertyDto);
        }



        public async Task<ActionResult<PropertyDetailedDto>> UpdateProperty(UpdatePropertyDto updatePropertyDto, string lastUser, Guid propertyId)
        {
            // validation
            await _updatePropertyValidator.ValidateAndThrowAsync(updatePropertyDto);

            await PropertyExists(propertyId);

            var oldProperty = await _propertiesRepo.GetByIdAsync(propertyId);

            var property = _mapper.Map<UpdatePropertyDto, Property>(updatePropertyDto);

            property.LastUpdateAt = DateTime.UtcNow;
            property.LastUpdateByUser = lastUser;
            property.Id = propertyId;

            if (!string.IsNullOrEmpty(property.CadastreRef))
            {
                string catUrl = await CreateCatastreURL(property.CadastreRef);
                property.CadastreUrl = catUrl;
            }

            property = await _propertiesRepo.UpdateAsync(property);

            _ = PerformAudit(property, propertyId);

            var propertyDto = _mapper.Map<Property, PropertyDetailedDto>(property);

            return new OkObjectResult(propertyDto);
        }

        public async Task<IActionResult> UpdatePropertyArchiveIdAsync(Guid propertyId, Guid archiveId, string username)
        {
            await PropertyExists(propertyId);

            await _propertiesRepo.UpdateArchiveIdAsync(propertyId, archiveId, username);
            return new NoContentResult();
        }

        public async Task PerformAudit(Property property, Guid propertyId)
        {
            Audit audit = new Audit();
            audit.Object = JsonSerializer.Serialize(property);
            audit.Id = propertyId;
            audit.ObjectType = "Property";

            string serializedAudit = JsonSerializer.Serialize(audit);
            MessageContract m = new MessageContract();
            m.Payload = serializedAudit;

            await _publishEndpoint.Publish<MessageContract>(m);
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
            var propertyDtos = results.Any() ? _mapper.Map<IEnumerable<PropertyDto>>(results) : Enumerable.Empty<PropertyDto>();
            return new OkObjectResult(propertyDtos);
        }

        public async Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser)
        {
            await PropertyExists(propertyId);

            var client = new OwnershipServiceClient();
            var ownerships = await client.GetOwnershipsByIdAsync(propertyId);

            if (ownerships!.Any())
                foreach (var ownership in ownerships!)
                {
                    var clientO = new OwnershipServiceClient();
                    await client.DeleteOwnershipAsync(ownership.Id);
                }

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

            var childs = await GetChildData(propertyId);

            propertyDto.ChildProperties = childs;

            return propertyDto;
        }

        private async Task<OwnerDto> GetContactData(Guid id)
        {
            var contactDto = new OwnerDto();

            var clientC = new ContactServiceClient();
            var contact = await clientC.GetContactByIdAsync(id);

            contactDto.Id = contact!.Id;
            contactDto.OwnerName = contact.FirstName + " " + contact.LastName;
            contactDto.OwnerType = "Contact";

            return contactDto;
        }

        private async Task CreateMainOwnership(Guid ownerId, string ownerType, Guid propertyId)
        {
            var ownershipDto = new OwnershipDto
            {
                OwnerId = ownerId,
                OwnerType = ownerType,
                PropertyId = propertyId,
                Share = 100,
                MainOwnership = true
            };

            var client = new OwnershipServiceClient();
            await client.CreateOwnershipAsync(ownershipDto);
        }

        private async Task<BasicPropertyDto> GetParentData(Guid id)
        {
            var propertyDto = new BasicPropertyDto();
            var property = await _propertiesRepo.GetByIdAsync(id);
            propertyDto.Id = property.Id;
            propertyDto.PropertyName = property.Name!;

            return propertyDto;
        }

        private async Task<List<PropertyDto>> GetChildData(Guid id)
        {

            var properties = await _propertiesRepo.GetPropertiesByParentIdAsync(id);
            var propertyDtOs = properties.Any() ? _mapper.Map<IEnumerable<PropertyDto>>(properties) : Enumerable.Empty<PropertyDto>();
            return propertyDtOs.ToList<PropertyDto>();
        }

        private async Task<string> CreateCatastreURL(string refCatastre)
        {
            Uri uri = new Uri($"http://ovc.catastro.meh.es/OVCServWeb/OVCWcfCallejero/COVCCallejero.svc/rest/Consulta_DNPRC?RefCat={refCatastre}");
            using (var client = new HttpClient())
            {
                var resp = await client.GetAsync(uri);
                if (!resp.IsSuccessStatusCode) return "";

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(await resp.Content.ReadAsStringAsync());

                var delNode = xml.GetElementsByTagName("cp")?.Item(0);
                var munNode = xml.GetElementsByTagName("cm")?.Item(0);

                if (delNode == null || munNode == null)
                {
                    // Handle the case where either "cp" or "cm" tags are missing in the XML
                    return "invalid";
                }

                var del = delNode.InnerText;
                var mun = munNode.InnerText;

                return $"https://www1.sedecatastro.gob.es/CYCBienInmueble/OVCConCiud.aspx?del={del}&mun={mun}&RefC={refCatastre}";
            }


        }
    }
}
