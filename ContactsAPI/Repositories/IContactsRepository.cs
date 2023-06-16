using MongoDB.Bson;
using MongoDB.Driver;
using ContactsAPI.Models;

namespace ContactsAPI.Repositories
{
    public interface IContactsRepository
    {
        Task<Contact> InsertOneAsync(Contact contact);
        Task<List<Contact>> GetAsync();
        Task<Contact> UpdateAsync(Contact contact);
        Task<UpdateResult> SetDeleteAsync(Guid contact, bool deleted);
        Task<Contact> GetContactByIdAsync(Guid id);

    }
}
