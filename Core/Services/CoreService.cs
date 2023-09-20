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
        private readonly PropertyServiceClient _pClient;
        private readonly OwnershipServiceClient _oClient;

        private readonly IHttpContextAccessor _contextAccessor;

        public CoreService(IBus bus,
            PropertyServiceClient pClient,
            OwnershipServiceClient oClient,
            IHttpContextAccessor contextAccessor)
        {
            _bus = bus;
            _pClient = pClient;
            _contextAccessor = contextAccessor;
            _oClient = oClient;
        }

        public async Task<string> CreateProperty(string requestBody)
        {
            string? property = await _pClient.CreateProperty(requestBody);

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
        public async Task<string> CreateFolder(string requestBody, string archiveId, IHttpContextAccessor contextAccessor)
        {
            var client = new DocumentsServiceClient(contextAccessor);
            string? document = await client.CreateFolder(requestBody, archiveId); 
            return document;
        }

        public async Task<string> CreateArchive(string requestBody, IHttpContextAccessor contextAccessor, string? type)
        {
            // TODO: Change from REST call to RabbitMQ message
            var client = new DocumentsServiceClient(contextAccessor);
            string? archive = await client.CreateArchive(requestBody, type);

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

            await SendMessageToArchiveQueue(new MessageContract() { Payload = property } );
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

        Task<string> ICoreService.CreateProperty(string requestBody)
        {
            throw new NotImplementedException();
        }

        Task<string> ICoreService.CreateCompany(string requestBody)
        {
            throw new NotImplementedException();
        }

        Task<string> ICoreService.CreateContact(string requestBody)
        {
            throw new NotImplementedException();
        }

        Task<string> ICoreService.CreateArchive(string requestBody, IHttpContextAccessor contextAccessor, string? type)
        {
            throw new NotImplementedException();
        }

        Task<string> ICoreService.CreateFolder(string requestBody, string archiveId, IHttpContextAccessor contextAccessor)
        {
            throw new NotImplementedException();
        }

        Task<object> ICoreService.GetContact(Guid Id, IHttpContextAccessor contextAccessor)
        {
            throw new NotImplementedException();
        }

        Task<object> ICoreService.GetProperty(Guid Id, IHttpContextAccessor contextAccessor)
        {
            throw new NotImplementedException();
        }

        Task<object> ICoreService.GetCompany(Guid Id, IHttpContextAccessor contextAccessor)
        {
            throw new NotImplementedException();
        }

        Task<object> ICoreService.GetContacts(bool includeDeleted, IHttpContextAccessor contextAccessor)
        {
            throw new NotImplementedException();
        }

        Task<object> ICoreService.GetProperties(bool includeDeleted, IHttpContextAccessor contextAccessor)
        {
            throw new NotImplementedException();
        }

        Task<object> ICoreService.GetCompanies(bool includeDeleted, IHttpContextAccessor contextAccessor)
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
