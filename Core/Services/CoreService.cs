using CoreAPI.Models;
using MassTransit;
using MessagingContracts;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class CoreService : ICoreService
    {
        private readonly IBus _bus;
        private readonly ILogger<CoreService> _logger;
        public CoreService(IBus bus, ILogger<CoreService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        // TODO: Create tenant when creating company or contact!


        public async Task<string> CreateProperty(string requestBody, IHttpContextAccessor contextAccessor)
        {
            var client = new PropertyServiceClient(contextAccessor);
            string? property = await client.CreateProperty(requestBody);

            if (string.IsNullOrEmpty(property)) throw new Exception("Property service response is empty");

            JsonDocument jsonDocument = JsonDocument.Parse(property);
            JsonElement root = jsonDocument.RootElement;

            string propertyId = root.GetProperty("id").GetString();
            string propertyName = root.GetProperty("name").GetString();

            var archivePayload = new
            {
                name = propertyName,
                id = propertyId
            };

            var archive = await CreateArchive(archivePayload, contextAccessor, "Property");

            return property;
        }

        [Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        public async Task<string> CreateFolder(string requestBody, string archiveId, IHttpContextAccessor contextAccessor)
        {
            var client = new DocumentsServiceClient(contextAccessor);
            string? document = await client.CreateFolder(requestBody, archiveId); 
            return document;
        }

        public async Task<string> CreateArchive(dynamic requestBody, IHttpContextAccessor contextAccessor, string? type)
        {
            // TODO: Change from REST call to RabbitMQ message
            var client = new DocumentsServiceClient(contextAccessor);
            string? archive = await client.CreateArchive(JsonSerializer.Serialize(requestBody), type);
            // Update object who caused the creation of the archive

            switch (type?.ToLowerInvariant())
            {
                case "property":
                    await new PropertyServiceClient(contextAccessor).UpdatePropertyArchive(requestBody.id, archive);
                    break;
                case "contact":
                    await new ContactServiceClient(contextAccessor).UpdateContactArchive(requestBody.id, archive);
                    break;
                case "company":
                    await new CompanyServiceClient(contextAccessor).UpdateCompanyArchive(requestBody.id, archive);
                    break;
                default:
                    _logger.LogWarning($"Invalid or absent type for archive {archive}!");
                    break;
            }

            return archive;
        }

        public async Task<object> GetContact(Guid Id, IHttpContextAccessor contextAccessor)
        {
            var clientC = new CompanyServiceClient(contextAccessor);
            var company = await clientC.GetCompanyByIdAsync(Id);
            await SendMessageToAuditQueue(new MessageContract() { Payload = company });

            return company;
        }

        public async Task<object> GetProperty(Guid Id, IHttpContextAccessor contextAccessor)
        {
            var client = new PropertyServiceClient(contextAccessor);
            var property = await client.GetPropertyByIdAsync(Id);
            await SendMessageToArchivePropertyQueue(new MessageContract() { Payload = property } );
            return property;
        }

        public async Task SendMessageToAuditQueue(MessageContract message)
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/audit1"));
            await sendEndpoint.Send<MessageContract>(message);
        }

        public async Task SendMessageToArchivePropertyQueue(MessageContract message)
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/aproperty2"));
            await sendEndpoint.Send<MessageContract>(message);
        }
    }
}
