using ContactsAPI.Contexts;
using ContactsAPI.Models;
using ContactsAPI.Repositories;
using MongoDB.Driver;

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
            var filter = Builders<Contact>.Filter.Eq(x => x.Deleted, false);

            return await _collection.Find(filter)
                .ToListAsync();
        }

        public async Task<Contact> GetContactByIdAsync(Guid id)
        {
            var filter = Builders<Contact>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            var filter = Builders<Contact>.Filter.Eq(actualContact => actualContact.Id, contact.Id);

            await _collection.ReplaceOneAsync(filter, contact);

            return contact;
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid contactId, bool deleted)
        {
            var filter = Builders<Contact>.Filter
                .Eq(actualContact => actualContact.Id, contactId);

            var update = Builders<Contact>.Update
                .Set(actualContact => actualContact.Deleted, deleted);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
