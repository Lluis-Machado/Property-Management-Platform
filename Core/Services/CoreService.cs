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

        public async Task<string> CreateProperty(string requestBody)
        {
            string? property = await _pClient.CreateProperty(requestBody);

            _logger.LogInformation($"CoreService - CreateProperty - Response: {property}");

            if (string.IsNullOrEmpty(property)) throw new Exception("Property service response is empty");

            var propertyId = GetPropertyFromJson(property,"id");
            var propertyName = GetPropertyFromJson(property, "name");

            var ownerType = GetPropertyFromJson(requestBody, "mainOwnerType");
            var ownerId = GetPropertyFromJson(requestBody, "mainOwnerId");

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

        public void RestoreVersion(string requestBody, string type)
        {
            switch (type)
            {
                case "Property":
                    UpdateProperty(requestBody);
                    break;
                case "Contact":
                    UpdateContact(requestBody);
                    break;
                case "Company":
                    UpdateCompany(requestBody);
                    break;
                default:
                    break;
            }
        }


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



        [Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        public async Task<string> CreateFolder(string requestBody, string archiveId)
        {
            string? document = await _docClient.CreateFolder(requestBody, archiveId);
            return document;
        }

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

        public async Task<object> GetContact(Guid Id)
        {
            var company = await _compClient.GetCompanyByIdAsync(Id);
            await SendMessageToAuditQueue(new MessageContract() { Payload = company });

            return company;
        }

        public async Task<object> GetProperty(Guid Id)
        {
            var property = await _pClient.GetPropertyByIdAsync(Id);
            await SendMessageToArchiveQueue(new MessageContract() { Payload = property });
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

        public async Task<string> CreateCompany(string requestBody)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateContact(string requestBody)
        {
            throw new NotImplementedException();
        }


        public async Task<object> GetCompany(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetContacts(bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetProperties(bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetCompanies(bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateProperty(string requestBody)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateCompany(string requestBody)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateContact(string requestBody)
        {
            throw new NotImplementedException();
        }




        private string GetPropertyFromJson(string json, string property)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            JsonElement root = jsonDocument.RootElement;

            return root.GetProperty(property).GetString();
        }



    }
}
