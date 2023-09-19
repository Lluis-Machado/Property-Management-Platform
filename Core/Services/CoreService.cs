using MassTransit;
using MessagingContracts;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class CoreService : ICoreService
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CoreService> _logger;

        public CoreService(IBus bus, IHttpContextAccessor contextAccessor, ILogger<CoreService> logger)
        {
            _bus = bus;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<string> CreateProperty(string requestBody)
        {
            string? property = await new PropertyServiceClient(_contextAccessor).CreateProperty(requestBody);

            if (string.IsNullOrEmpty(property)) throw new Exception("Property service response is empty");

            JsonDocument jsonDocument = JsonDocument.Parse(property);
            JsonElement root = jsonDocument.RootElement;

            string propertyId = root.GetProperty("id").GetString();
            string propertyName = root.GetProperty("name").GetString();

            var archivePayload = new
            {
                name = propertyName,
                id = propertyId,
                type = "property"
            };
            _ = SendMessageToArchiveQueue(new MessageContract() { Payload = archivePayload.ToString() });

            _ = CreateArchive(archivePayload, "Property");

            return property;
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
    }
}
