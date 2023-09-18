using ContactsAPI.DTOs;
using ContactsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ContactsAPI.Repositories
{
    public interface IContactsRepository
    {
        Task<Contact> InsertOneAsync(Contact contact);
        Task<List<Contact>> GetAsync(bool includeDeleted = false);
        Task<IEnumerable<Contact>> SearchAsync(string query);
        Task<Contact> UpdateAsync(Contact contact);
        Task<UpdateResult> UpdateContactArchiveIdAsync(Guid contactId, Guid archiveId, string lastUser);
        Task<UpdateResult> SetDeleteAsync(Guid contact, bool deleted, string lastUser);
        Task<Contact?> GetContactByIdAsync(Guid contactId);
        Task<bool> CheckIfNIEUnique(string NIE, Guid? contactId);
    }
}