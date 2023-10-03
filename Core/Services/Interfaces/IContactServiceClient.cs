﻿using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IContactServiceClient
    {

        Task<JsonDocument> CreateContactAsync(string requestBody);
        Task<JsonDocument?> GetContactByIdAsync(Guid id);
        Task<JsonDocument?> UpdateContactArchive(string contactId, string archiveId);
        Task<JsonDocument?> UpdateContact(Guid contactId, string requestBody);
        Task DeleteContact(Guid contactId);
    }
}
