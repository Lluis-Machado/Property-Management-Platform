using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CoreAPI.Services;

public class ContactServiceClient : IContactServiceClient
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IBaseClientService _baseClient;

    public ContactServiceClient(IHttpContextAccessor contextAccessor, IBaseClientService baseClient)
    {
        _contextAccessor = contextAccessor;
        _baseClient = baseClient;
    }

    public async Task<JsonDocument?> GetContactByIdAsync(Guid id)
    {
        return await _baseClient.ReadAsync($"contacts/contacts/{id}");
    }

    public async Task<JsonDocument?> UpdateContactArchive(string contactId, string archiveId)
    {
        return await _baseClient.UpdateAsync($"contacts/contacts/{contactId}/{archiveId}");
    }

    // Get contact, check if updated object has different name/surname
    // If name is different, update the archive as well
    public async Task<JsonDocument?> UpdateContact(Guid contactId, string requestBody)
    {
        // Body validation
        if (string.IsNullOrEmpty(requestBody)) throw new BadHttpRequestException("Request body format not valid");

        // Get pre-update contact information
        JsonDocument? currentContact = await GetContactByIdAsync(contactId);
        if (currentContact is null) throw new Exception($"Update failed - Contact with id {contactId} not found");
        string currentFirstName = currentContact.RootElement.GetProperty("firstName").GetString() ?? "";
        string currentLastName = currentContact.RootElement.GetProperty("lastName").GetString() ?? "";
        string currentName = currentFirstName != "" && currentLastName != "" ? $"{currentLastName}, {currentFirstName}" : "";

        // Get post-update contact name
        JsonDocument body = JsonSerializer.Deserialize<JsonDocument>(requestBody);
        string requestFirstName = body.RootElement.GetProperty("firstName").GetString() ?? "";
        string requestLastName = body.RootElement.GetProperty("lastName").GetString() ?? "";
        string requestName = requestFirstName != "" && requestLastName != "" ? $"{requestLastName}, {requestFirstName}" : "";

        // Perform company update
        var contactUpdate = await _baseClient.UpdateAsync($"contacts/contacts/{contactId}", requestBody);

        // If the name has changed, perform Archive name change
        if (currentName != requestName && currentName != "")
        {
            await _baseClient.UpdateAsync($"documents/archives/{currentContact.RootElement.GetProperty("archiveId").GetString()}&newName={Uri.EscapeDataString(requestName)}");
        }

        return contactUpdate;
    }

    public async Task DeleteContact(Guid contactId)
    {
        bool res = await _baseClient.DeleteAsync($"contacts/contacts/{contactId}");
        if (!res) throw new Exception($"Unknown error deleting contact {contactId}");
    }



}