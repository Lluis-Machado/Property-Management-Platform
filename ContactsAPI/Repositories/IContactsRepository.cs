﻿using ContactsAPI.Models;
using MongoDB.Driver;

namespace ContactsAPI.Repositories
{
    public interface IContactsRepository
    {
        Task<Contact> InsertOneAsync(Contact contact);
        Task<List<Contact>> GetAsync(bool includeDeleted = false);
        Task<Contact> UpdateAsync(Contact contact);
        Task<UpdateResult> SetDeleteAsync(Guid contact, bool deleted, string lastUser);
        Task<Contact> GetContactByIdAsync(Guid contactId);
        Task<bool> CheckIfNIEUnique(string NIE, Guid? contactId);
    }
}