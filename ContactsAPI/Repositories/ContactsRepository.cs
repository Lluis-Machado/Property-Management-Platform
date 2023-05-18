using MongoDB.Driver;
using ContactsAPI.Contexts;
using ContactsAPI.Models;
using ContactsAPI.Repositories;

namespace ContactsAPI.Services
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly IMongoCollection<Contact> _collection;
        public ContactsRepository(MongoContext context)
        {
            var database = context.GetDataBase("contacts");
            _collection = database.GetCollection<Contact>("contacts");
        }

        public async Task<Contact> InsertOneAsync(Contact contact)
        {
            await _collection.InsertOneAsync(contact);
            return contact;
        }

        public async Task<List<Contact>> GetAsync()
        {
            var filter = Builders<Contact>.Filter.Empty;

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<UpdateResult> UpdateAsync(Contact contact)
        {
            var filter = Builders<Contact>.Filter
                .Eq(actualContact => actualContact._id, contact._id);

            var update = Builders<Contact>.Update
                .Set(actualContact => actualContact.Name, contact.Name)
                .Set(actualContact => actualContact.Address, contact.Address);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid contactId, bool deleted)
        {
            var filter = Builders<Contact>.Filter
                .Eq(actualContact => actualContact._id, contactId);

            var update = Builders<Contact>.Update
                .Set(actualContact => actualContact.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
