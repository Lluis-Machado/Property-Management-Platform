using MassTransit;
using MessagingContracts;
using System;
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
            JsonDocument? property = await _pClient.CreateProperty(requestBody);

            _logger.LogInformation($"CoreService - CreateProperty - Response: {property?.RootElement.ToString()}");

            if (property is null || string.IsNullOrEmpty(property.RootElement.GetProperty("id").GetString())) throw new Exception("Property service response is empty");


            string? propertyId, propertyName, ownerType, ownerId;

            propertyId = GetPropertyFromJson(property, "id");
            propertyName = GetPropertyFromJson(property, "name");
            ownerType = GetPropertyFromJson(property, "mainOwnerType");
            ownerId = GetPropertyFromJson(property, "mainOwnerId");

            
            var archivePayload = new
            {
                name = propertyName,
                id = propertyId,
                type = "property"
            };
            if (!String.IsNullOrEmpty(ownerType))
                await CreateMainOwnership(ownerId, ownerType, propertyId);

            //await SendMessageToArchiveQueue(new MessageContract() { Payload = JsonSerializer.Serialize(archivePayload) });

            await CreateArchive(JsonSerializer.Serialize(archivePayload), "Property", propertyId);

            return property;
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

            await _oClient.CreateOwnership(JsonSerializer.Serialize(ownershipDto));
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
            JsonDocument? archive = await _docClient.CreateArchive(requestBody, type, id);
            if (archive is null) throw new Exception($"Error creating archive for {type?.ToLower()}");

            // Update object who caused the creation of the archive
            switch (type?.ToLowerInvariant())
            {
                case "property":
                    await _pClient.UpdatePropertyArchive(GetPropertyFromJson(requestBody, "id"), archive.RootElement.GetProperty("id").GetString()!);
                    break;
                case "contact":
                    await _contClient.UpdateContactArchive(GetPropertyFromJson(requestBody, "id"), archive.RootElement.GetProperty("id").GetString()!);
                    break;
                case "company":
                    await _compClient.UpdateCompanyArchive(GetPropertyFromJson(requestBody, "id"), archive.RootElement.GetProperty("id").GetString()!);
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

        public async Task<JsonDocument?> CreateCompany(string requestBody)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonDocument?> CreateContact(string requestBody)
        {
            throw new NotImplementedException();
        }


        public async Task<JsonDocument?> GetCompany(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonDocument[]?> GetContacts(bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonDocument[]?> GetProperties(bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonDocument[]?> GetCompanies(bool includeDeleted)
        {
            throw new NotImplementedException();
        }




        public async Task<JsonDocument?> UpdateProperty(Guid propertyId, string requestBody)
        {
            return await _pClient.UpdateProperty(propertyId, requestBody);
        }

        public async Task<JsonDocument?> UpdateCompany(Guid companyId, string requestBody)
        {
            return await _compClient.UpdateCompany(companyId, requestBody);
        }

        public async Task<JsonDocument?> UpdateContact(Guid contactId, string requestBody)
        {
            return await _contClient.UpdateContact(contactId, requestBody);
        }


        public async Task DeleteProperty(Guid propertyId)
        {
            // TODO: Check related/child properties
            JsonDocument? property = await _pClient.GetPropertyByIdAsync(propertyId);


            if (property is null) throw new Exception($"Property with ID {propertyId} not found");

            string? archiveIdstr = GetPropertyFromJson(property, "archiveId");
            if (string.IsNullOrEmpty(archiveIdstr)) throw new Exception($"Archive ID not found in property {propertyId} {GetPropertyFromJson(property, "name")}");

            Guid archiveId = Guid.Parse(archiveIdstr);
            if (archiveId == Guid.Empty) throw new Exception($"Invalid Guid for Archive ID - Property {propertyId} {GetPropertyFromJson(property, "name")}");

            Task[] tasks = { _pClient.DeleteProperty(propertyId), _docClient.DeleteArchive(archiveId) };

            // TODO: Delete ownerships with propertyId


            Task.WaitAll(tasks);
        }

        public async Task DeleteContact(Guid contactId) {
            // TODO: Check if any ownerships exist
        


        }

        public async Task DeleteCompany(Guid companyId) {
            // TODO: Check if any ownerships exist
        
        }





        private string GetPropertyFromJson(string json, string property)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            JsonElement root = jsonDocument.RootElement;

            return root.GetProperty(property).GetString() ?? "";
        }

        private string? GetPropertyFromJson(JsonDocument json, string property)
        {
            try
            {
                return json.RootElement.GetProperty(property).GetString();
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }



    }
}
