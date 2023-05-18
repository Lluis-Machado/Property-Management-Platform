using MongoDB.Bson;
using MongoDB.Driver;
using ContactsAPI.Models;

namespace ContactsAPI.Repositories
{
    public interface IContactsRepository
    {
        Task<Contact> InsertOneAsync(Contact contact);
        Task<List<Contact>> GetAsync();
        Task<UpdateResult> UpdateAsync(Contact contact);
        Task<UpdateResult> SetDeleteAsync(Guid contact, bool deleted);
    }
}
