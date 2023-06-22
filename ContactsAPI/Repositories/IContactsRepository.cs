using ContactsAPI.Models;
using MongoDB.Driver;

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
