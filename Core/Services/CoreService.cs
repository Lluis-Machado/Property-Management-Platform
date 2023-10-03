using CoreAPI.Utils;
using MassTransit;
using MessagingContracts;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class CoreService : ICoreService
    {
        private readonly IBus _bus;
        private readonly IPropertyServiceClient _pClient;
        private readonly IOwnershipServiceClient _oClient;
        private readonly ICompanyServiceClient _compClient;
        private readonly IDocumentsServiceClient _docClient;
        private readonly IContactServiceClient _contClient;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CoreService> _logger;

        public CoreService(IBus bus,
            IPropertyServiceClient pClient,
            IOwnershipServiceClient oClient,
            ICompanyServiceClient compClient,
            IDocumentsServiceClient docClient,
            IContactServiceClient contClient,
            ILogger<CoreService> logger,
            IHttpContextAccessor contextAccessor)
        {
            _bus = bus;
            _pClient = pClient;
            _oClient = oClient;
            _compClient = compClient;
            _docClient = docClient;
            _contClient = contClient;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public async Task<JsonDocument> CreateProperty(string requestBody)
        {
            JsonDocument? property = await _pClient.CreatePropertyAsync(requestBody);

            _logger.LogInformation($"CoreService - CreateProperty - Response: {property?.RootElement.ToString()}");

            if (property is null || string.IsNullOrEmpty(property.RootElement.GetProperty("id").GetString())) throw new Exception("Property service response is empty");


            string? propertyId, propertyName, ownerType, ownerId;

            propertyId = GetPropertyFromJson(property, "id");
            propertyName = GetPropertyFromJson(property, "name");
            ownerType = GetPropertyFromJson(property, "mainOwnerType");
            ownerId = GetPropertyFromJson(property, "mainOwnerId");


            var archivePayload = new
            {
                Name = propertyName
            };

            if (!String.IsNullOrEmpty(ownerType))
                await CreateMainOwnership(ownerId, ownerType, propertyId);

            //await SendMessageToArchiveQueue(new MessageContract() { Payload = JsonSerializer.Serialize(archivePayload) });

            await CreateArchive(JsonSerializer.Serialize(archivePayload), "Property", propertyId);

            return property;
        }

        public async Task<JsonDocument> CreateContact(string requestBody)
        {
            JsonDocument? contact = await _contClient.CreateContactAsync(requestBody);

            _logger.LogInformation($"CoreService - CreateContact - Response: {contact?.RootElement.ToString()}");

            if (contact is null || string.IsNullOrEmpty(contact.RootElement.GetProperty("id").GetString())) throw new Exception("Contact service response is empty");


            string? contactId, contactName, ownerType, ownerId;

            contactId = GetPropertyFromJson(contact, "id");
            contactName = GetPropertyFromJson(contact, "lastName") + ", " + GetPropertyFromJson(contact, "firstName");
            ownerType = GetPropertyFromJson(contact, "mainOwnerType");
            ownerId = GetPropertyFromJson(contact, "mainOwnerId");


            var archivePayload = new
            {
                Name = contactName
            };
            if (!String.IsNullOrEmpty(ownerType))
                await CreateMainOwnership(ownerId, ownerType, contactId);

            //await SendMessageToArchiveQueue(new MessageContract() { Payload = JsonSerializer.Serialize(archivePayload) });

            await CreateArchive(JsonSerializer.Serialize(archivePayload), "Contact", contactId);

            return contact;
        }

        public async Task<JsonDocument> CreateCompany(string requestBody)
        {
            JsonDocument? company = await _compClient.CreateCompanyAsync(requestBody);

            _logger.LogInformation($"CoreService - CreateCompany - Response: {company?.RootElement.ToString()}");

            if (company is null || string.IsNullOrEmpty(company.RootElement.GetProperty("id").GetString())) throw new Exception("Company service response is empty");


            string? companyId, companyName, ownerType, ownerId;

            companyId = GetPropertyFromJson(company, "id");
            companyName = GetPropertyFromJson(company, "name");
            ownerType = GetPropertyFromJson(company, "mainOwnerType");
            ownerId = GetPropertyFromJson(company, "mainOwnerId");


            var archivePayload = new
            {
                Name = companyName
            };
            if (!String.IsNullOrEmpty(ownerType))
                await CreateMainOwnership(ownerId, ownerType, companyId);

            //await SendMessageToArchiveQueue(new MessageContract() { Payload = JsonSerializer.Serialize(archivePayload) });

            await CreateArchive(JsonSerializer.Serialize(archivePayload), "Company", companyId);

            return company;
        }

        // TODO: Update so that we can also pass the object Guid
        //public void RestoreVersion(string requestBody, string type)
        //{
        //    switch (type)
        //    {
        //        case "Property":
        //            UpdateProperty(requestBody);
        //            break;
        //        case "Contact":
        //            UpdateContact(requestBody);
        //            break;
        //        case "Company":
        //            UpdateCompany(requestBody);
        //            break;
        //        default:
        //            break;
        //    }
        //}


        private async Task CreateMainOwnership(string ownerId, string ownerType, string propertyId)
        {
            _logger.LogDebug($"CoreService - CreateMainOwnership | OwnerId: {ownerId} | OwnerType: {ownerType} | PropertyId: {propertyId}");

            var ownershipDto = new
            {
                OwnerId = ownerId,
                OwnerType = ownerType,
                PropertyId = propertyId,
                Share = 100,
                MainOwnership = true
            };

            await _oClient.CreateOwnershipAsync(JsonSerializer.Serialize(ownershipDto));
        }



        //[Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        //public async Task<string> CreateFolder(string requestBody, string archiveId)
        //{
        //    string? document = await _docClient.CreateFolder(requestBody, archiveId);
        //    return document;
        //}

        public async Task<JsonDocument> CreateArchive(string requestBody, string? type, string? id)
        {
            // TODO: Change from REST call to RabbitMQ message
            _logger.LogInformation($"Sending archive creation request");
            JsonDocument? archive = await _docClient.CreateArchiveAsync(requestBody, type, id);
            if (archive is null) throw new Exception($"Error creating archive for {type?.ToLower()}");

            // Update object who caused the creation of the archive
            switch (type?.ToLowerInvariant())
            {
                case "property":
                    await _pClient.UpdatePropertyArchiveAsync(GetPropertyFromJson(requestBody, "id"), archive.RootElement.GetProperty("id").GetString()!);
                    break;
                case "contact":
                    await _contClient.UpdateContactArchiveAsync(GetPropertyFromJson(requestBody, "id"), archive.RootElement.GetProperty("id").GetString()!);
                    break;
                case "company":
                    await _compClient.UpdateCompanyArchiveAsync(GetPropertyFromJson(requestBody, "id"), archive.RootElement.GetProperty("id").GetString()!);
                    break;
                default:
                    _logger.LogWarning($"Invalid or absent type for archive {archive.RootElement.GetProperty("id").GetString()}!");
                    break;
            }

            return archive;
        }

        public async Task<JsonDocument?> GetContact(Guid Id)
        {
            var company = await _compClient.GetCompanyByIdAsync(Id);
            await SendMessageToAuditQueue(new MessageContract() { Payload = company?.RootElement.ToString() });

            return company;
        }

        public async Task<JsonDocument?> GetProperty(Guid Id)
        {
            var property = await _pClient.GetPropertyByIdAsync(Id);
            await SendMessageToArchiveQueue(new MessageContract() { Payload = property?.RootElement.ToString() });
            return property;
        }

        public async Task SendMessageToAuditQueue(MessageContract message)
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/audit1"));
            await sendEndpoint.Send(message);
        }

        public async Task SendMessageToArchiveQueue(MessageContract message)
        {
            _logger.LogInformation($"Sending message to archive queue");
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/archive"));
            await sendEndpoint.Send(message);
        }


        public async Task<JsonDocument?> UpdateProperty(Guid propertyId, string requestBody)
        {
            return await _pClient.UpdatePropertyAsync(propertyId, requestBody);
        }

        public async Task<JsonDocument?> UpdateCompany(Guid companyId, string requestBody)
        {
            return await _compClient.UpdateCompanyAsync(companyId, requestBody);
        }

        public async Task<JsonDocument?> UpdateContact(Guid contactId, string requestBody)
        {
            return await _contClient.UpdateContactAsync(contactId, requestBody);
        }


        public async Task DeleteProperty(Guid propertyId)
        {
            _logger.LogInformation($"CoreService - Beginning delete of property {propertyId}");

            // Check if there are any ownerships left

            JsonDocument? ownerships = await _oClient.GetOwnershipByIdAsync(propertyId, "property");

            _logger.LogInformation($"Got ownership information: {JsonSerializer.Serialize(ownerships)} - IsNull? {ownerships is null} - ArrLength? {ownerships?.RootElement.GetArrayLength()} || ArrLength is 0? {ownerships?.RootElement.GetArrayLength() == 0}");

            if (ownerships?.RootElement.GetArrayLength() != 0)
            {
                _logger.LogError($"CoreService - Found Ownerships for property {propertyId}! | {JsonSerializer.Serialize(ownerships)}");
                throw new OwnershipExistsException(ownerships!);
            }

            // Check if there are child properties

            JsonDocument? property = await _pClient.GetPropertyByIdAsync(propertyId);

            if (property is null) throw new Exception($"Property with ID {propertyId} not found");

            string? childProps = GetPropertyFromJson(property, "childProperties");

            if (!string.IsNullOrEmpty(childProps))
            {
                _logger.LogError($"CoreService - Found child properties for property {propertyId}! | {childProps}");
                throw new ChildPropertiesExistException(property.RootElement.GetProperty("childProperties"));
            }

            string? archiveIdstr = GetPropertyFromJson(property, "archiveId");
            if (string.IsNullOrEmpty(archiveIdstr)) throw new Exception($"Archive ID not found in property {propertyId} {GetPropertyFromJson(property, "name")}");

            Guid archiveId = Guid.Parse(archiveIdstr);
            if (archiveId == Guid.Empty) throw new Exception($"Invalid Guid for Archive ID - Property {propertyId} {GetPropertyFromJson(property, "name")}");

            Task[] tasks = { _pClient.DeletePropertyAsync(propertyId), _docClient.DeleteArchiveAsync(archiveId) };

            Task.WaitAll(tasks);
        }

        public async Task DeleteContact(Guid contactId)
        {

            _logger.LogInformation($"CoreService - Beginning delete of contact {contactId}");

            // Check if there are any ownerships left

            JsonDocument? ownerships = await _oClient.GetOwnershipByIdAsync(contactId, "contact");

            _logger.LogInformation($"Got ownership information: {JsonSerializer.Serialize(ownerships)} - IsNull? {ownerships is null} - ArrLength? {ownerships?.RootElement.GetArrayLength()} || ArrLength is 0? {ownerships?.RootElement.GetArrayLength() == 0}");

            if (ownerships?.RootElement.GetArrayLength() != 0)
            {
                _logger.LogError($"CoreService - Found Ownerships for contact {contactId}! | {JsonSerializer.Serialize(ownerships)}");
                throw new OwnershipExistsException(ownerships!);
            }

            JsonDocument? contact = await _contClient.GetContactByIdAsync(contactId);

            if (contact is null) throw new Exception($"Contact with ID {contactId} not found");

            string? archiveIdstr = GetPropertyFromJson(contact, "archiveId");
            if (string.IsNullOrEmpty(archiveIdstr)) throw new Exception($"Archive ID not found in contact {contactId} {GetPropertyFromJson(contact, "name")}");

            Guid archiveId = Guid.Parse(archiveIdstr);
            if (archiveId == Guid.Empty) throw new Exception($"Invalid Guid for Archive ID - Contact {contactId} {GetPropertyFromJson(contact, "name")}");

            Task[] tasks = { _contClient.DeleteContactAsync(contactId), _docClient.DeleteArchiveAsync(archiveId) };

            Task.WaitAll(tasks);
        }

        public async Task DeleteCompany(Guid companyId)
        {

            _logger.LogInformation($"CoreService - Beginning delete of company {companyId}");

            // Check if there are any ownerships left

            JsonDocument? ownerships = await _oClient.GetOwnershipByIdAsync(companyId, "company");

            _logger.LogInformation($"Got ownership information: {JsonSerializer.Serialize(ownerships)} - IsNull? {ownerships is null} - ArrLength? {ownerships?.RootElement.GetArrayLength()} || ArrLength is 0? {ownerships?.RootElement.GetArrayLength() == 0}");

            if (ownerships?.RootElement.GetArrayLength() != 0)
            {
                _logger.LogError($"CoreService - Found Ownerships for company {companyId}! | {JsonSerializer.Serialize(ownerships)}");
                throw new OwnershipExistsException(ownerships!);
            }

            JsonDocument? company = await _compClient.GetCompanyByIdAsync(companyId);

            if (company is null) throw new Exception($"Company with ID {companyId} not found");

            string? archiveIdstr = GetPropertyFromJson(company, "archiveId");
            if (string.IsNullOrEmpty(archiveIdstr)) throw new Exception($"Archive ID not found in company {companyId} {GetPropertyFromJson(company, "name")}");

            Guid archiveId = Guid.Parse(archiveIdstr);
            if (archiveId == Guid.Empty) throw new Exception($"Invalid Guid for Archive ID - Company {companyId} {GetPropertyFromJson(company, "name")}");

            Task[] tasks = { _compClient.DeleteCompanyAsync(companyId), _docClient.DeleteArchiveAsync(archiveId) };

            Task.WaitAll(tasks);


        }





        private string GetPropertyFromJson(string json, string property)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            JsonElement root = jsonDocument.RootElement;

            return root.GetProperty(property).GetString() ?? "";
        }

        public static string? GetPropertyFromJson(JsonDocument json, string property)
        {
            try
            {
                var prop = json.RootElement.GetProperty(property);

                if (prop.ValueKind == JsonValueKind.Array) return prop.ToString();
                else return prop.GetString();
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }



    }
}
