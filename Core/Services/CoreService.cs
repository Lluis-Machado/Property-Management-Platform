using CoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class CoreService : ICoreService
    {
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

            var archive = await CreateArchive(JsonSerializer.Serialize(archivePayload), contextAccessor, "Property");

            return property;
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
            return company;
        }

        public async Task<object> GetProperty(Guid Id, IHttpContextAccessor contextAccessor)
        {
            var client = new PropertyServiceClient(contextAccessor);
            var property = await client.GetPropertyByIdAsync(Id);
            return property;
        }

        public Task FixOwnerships()
        {
            
            List<Ownership> ownerships = new();
            List<Guid> brokenOwnerships = ownerships.Where(x => x.Id == Guid.Empty).Select(x => x.Id).ToList();
            brokenOwnerships = ownerships.Where(x => x.PropertyId == Guid.Empty).Select(x => x.Id).ToList();
            brokenOwnerships = ownerships.Where(x => x.OwnerId == Guid.Empty).Select(x => x.Id).ToList();
            brokenOwnerships = ownerships.Where(x => x.OwnerType.ToLower() != "contacts" || x.OwnerType.ToLower() != "company").Select(x => x.Id).ToList();



            List<Guid> properties = ownerships.Select(x => x.PropertyId).ToList();
            List<Guid> companies = ownerships.Where(x => x.OwnerType.ToLower() == "company").Select(x => x.OwnerId).ToList();
            List<Guid> contacts = ownerships.Where(x => x.OwnerType.ToLower() == "contact").Select(x => x.OwnerId).ToList();


            return Task.CompletedTask;

        }
    }
}
