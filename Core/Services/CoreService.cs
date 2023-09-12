using CoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CoreAPI.Services
{
    public class CoreService : ICoreService
    {
        public async Task<string> CreateProperty(string requestBody)
        {
            var client = new PropertyServiceClient();
            string? property = await client.CreateProperty(requestBody);

            JsonDocument jsonDocument = JsonDocument.Parse(property);
            JsonElement root = jsonDocument.RootElement;

            string propertyId = root.GetProperty("id").GetString();
            string propertyName = root.GetProperty("name").GetString();

            string archivePayload = $"{{\"name\":\"{propertyName}\"}}";
            var archive = await CreateArchive(archivePayload);

            JsonDocument jsonDocumentA = JsonDocument.Parse(archive);

            // Get the root element of the JSON document
            JsonElement rootA = jsonDocumentA.RootElement;

            // Extract the ID value
            string archiveId = rootA.GetProperty("id").GetString();

            string[] folderNames = { "Invoices", "Documents", "Other" };

            foreach (var name in folderNames)
            {
                string folderPayload = $"{{\"name\":\"{name}\"}}";
                _ = await CreateFolder(folderPayload, archiveId);
            }
            return property;
        }

        public async Task<string> CreateFolder(string requestBody, string archiveId)
        {
            var client = new DocumentsServiceClient();
            string? document = await client.CreateFolder(requestBody, archiveId); 
            return document;
        }

        public async Task<string> CreateArchive(string requestBody)
        {
            var client = new DocumentsServiceClient();
            string? archive = await client.CreateArchive(requestBody);
            return archive;
        }

        public async Task<object> GetContact(Guid Id)
        {
            var clientC = new CompanyServiceClient();
            var company = await clientC.GetCompanyByIdAsync(Id);
            return company;
        }

        public async Task<object> GetProperty(Guid Id)
        {
            var client = new PropertyServiceClient();
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




            return null;



        }
    }
}
