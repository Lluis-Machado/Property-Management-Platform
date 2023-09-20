using MassTransit;
using MessagingContracts;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class CoreService : ICoreService
    {
        private readonly IBus _bus;
        private readonly PropertyServiceClient _pClient;
        private readonly OwnershipServiceClient _oClient;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CoreService> _logger;

        public CoreService(IBus bus,
            PropertyServiceClient pClient,
            OwnershipServiceClient oClient,
            IHttpContextAccessor contextAccessor)
        {
            _bus = bus;
            _pClient = _pClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<string> CreateProperty(string requestBody)
        {
            string? property = await new PropertyServiceClient(_contextAccessor).CreateProperty(requestBody);

            if (string.IsNullOrEmpty(property)) throw new Exception("Property service response is empty");

            var propertyId = GetPropertyFromJson(property,"Id");
            var propertyName = GetPropertyFromJson(property, "Name");

            var ownerType = GetPropertyFromJson(requestBody, "MainOwnerType");
            var ownerId = GetPropertyFromJson(requestBody, "MainOwnerId");

            var archivePayload = new
            {
                name = propertyName,
                id = propertyId,
                type = "property"
            };
            if (!String.IsNullOrEmpty(ownerType))
                await CreateMainOwnership(ownerId, ownerType, propertyId);
            /* await SendMessageToArchiveQueue(new MessageContract() { Payload = archivePayload.ToString() });

             var archive = await CreateArchive(JsonSerializer.Serialize(archivePayload), _contextAccessor, "Property");*/

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

        private string GetPropertyFromJson(string json, string property)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            JsonElement root = jsonDocument.RootElement;

            return root.GetProperty(property).GetString();
        }

        private async Task CreateMainOwnership(string ownerId, string ownerType, string propertyId)
        {
            var ownershipDto = new 
            {
                OwnerId = ownerId,
                OwnerType = ownerType,
                PropertyId = propertyId,
                Share = 100,
                MainOwnership = true
            };

           await _oClient.CreateOwnership(ownershipDto.ToString());
        }



        [Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        public async Task<string> CreateFolder(string requestBody, string archiveId)
        {
            var client = new DocumentsServiceClient(_contextAccessor);
            string? document = await client.CreateFolder(requestBody, archiveId);
            return document;
        }

        public async Task<string> CreateArchive(dynamic requestBody, string? type)
        {
            // TODO: Change from REST call to RabbitMQ message
            var client = new DocumentsServiceClient(_contextAccessor);
            string? archive = await client.CreateArchive(JsonSerializer.Serialize(requestBody), type);
            // Update object who caused the creation of the archive

            switch (type?.ToLowerInvariant())
            {
                case "property":
                    await new PropertyServiceClient(_contextAccessor).UpdatePropertyArchive(requestBody.id, archive);
                    break;
                case "contact":
                    await new ContactServiceClient(_contextAccessor).UpdateContactArchive(requestBody.id, archive);
                    break;
                case "company":
                    await new CompanyServiceClient(_contextAccessor).UpdateCompanyArchive(requestBody.id, archive);
                    break;
                default:
                    _logger.LogWarning($"Invalid or absent type for archive {archive}!");
                    break;
            }

            return archive;
        }

        public async Task<object> GetContact(Guid Id)
        {
            var clientC = new CompanyServiceClient(_contextAccessor);
            var company = await clientC.GetCompanyByIdAsync(Id);
            await SendMessageToAuditQueue(new MessageContract() { Payload = company });

            return company;
        }

        public async Task<object> GetProperty(Guid Id)
        {
            var client = new PropertyServiceClient(_contextAccessor);
            var property = await client.GetPropertyByIdAsync(Id);
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

        public async Task<string> CreateArchive(string requestBody, string? type)
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
    }
}
