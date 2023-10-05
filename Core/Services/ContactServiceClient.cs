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

    public async Task<JsonDocument?> UpdateContactArchiveAsync(string contactId, string archiveId)
    {
        return await _baseClient.UpdateAsync($"contacts/contacts/{contactId}/{archiveId}");
    }

    public async Task<JsonDocument> CreateContactAsync(string requestBody)
    {
        JsonDocument? contact = await _baseClient.CreateAsync($"contacts/contacts", requestBody);
        if (contact is null) throw new Exception("Error creating contact!");

        return contact;
    }

    // Get contact, check if updated object has different name/surname
    // If name is different, update the archive as well
    public async Task<JsonDocument?> UpdateContactAsync(Guid contactId, string requestBody)
    {
        // Body validation
        if (string.IsNullOrEmpty(requestBody)) throw new BadHttpRequestException("Request body format not valid");

        // Get pre-update contact information
        JsonDocument? currentContact = await GetContactByIdAsync(contactId);
        if (currentContact is null) throw new Exception($"Update failed - Contact with id {contactId} not found");
        string currentFirstName = CoreService.GetPropertyFromJson(currentContact, "firstName") ?? "";
        string currentLastName = CoreService.GetPropertyFromJson(currentContact, "lastName") ?? "";
        string currentName = currentFirstName != "" && currentLastName != "" ? $"{currentLastName}, {currentFirstName}" : "";

        // Get post-update contact name
        JsonDocument body = JsonSerializer.Deserialize<JsonDocument>(requestBody);
        string requestFirstName = CoreService.GetPropertyFromJson(body, "firstName") ?? "";
        string requestLastName = CoreService.GetPropertyFromJson(body, "lastName") ?? "";
        string requestName = requestFirstName != "" && requestLastName != "" ? $"{requestLastName}, {requestFirstName}" : "";

        // Perform contact update
        var contactUpdate = await _baseClient.UpdateAsync<string>($"contacts/contacts/{contactId}", requestBody);

        // If the name has changed, perform Archive name change
        if (currentName != requestName && currentName != "")
        {
            await _baseClient.UpdateAsync($"documents/archives/{CoreService.GetPropertyFromJson(currentContact, "archiveId")}?newName={Uri.EscapeDataString(requestName)}");
        }

        return contactUpdate;
    }

    public async Task DeleteContactAsync(Guid contactId)
    {
        bool res = await _baseClient.DeleteAsync($"contacts/contacts/{contactId}");
        if (!res) throw new Exception($"Unknown error deleting contact {contactId}");
    }



}