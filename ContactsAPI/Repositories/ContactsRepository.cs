using ContactsAPI.Contexts;
using ContactsAPI.Models;
using MongoDB.Driver;

namespace ContactsAPI.Repositories
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

        public async Task<List<Contact>> GetAsync(bool includeDeleted = false)
        {
            FilterDefinition<Contact> filter;

            if (includeDeleted)
            {
                filter = Builders<Contact>.Filter.Empty;
            }
            else
            {
                filter = Builders<Contact>.Filter.Eq(x => x.Deleted, false);
            }

            var result = await _collection.Find(filter).ToListAsync();
            return result;
        }


        public async Task<Contact> GetContactByIdAsync(Guid id)
        {
            var filter = Builders<Contact>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckContactIfAnyExistAsync(Guid id)
        {
            var filter = Builders<Contact>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .AnyAsync();
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            var filter = Builders<Contact>.Filter.Eq(actualContact => actualContact.Id, contact.Id);

            await _collection.ReplaceOneAsync(filter, contact);

            return contact;
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid contactId, bool deleted, string lastUser)
        {
            var filter = Builders<Contact>.Filter
                .Eq(actualContact => actualContact.Id, contactId);

            var update = Builders<Contact>.Update
                .Set(actualContact => actualContact.Deleted, deleted)
                .Set(actualContact => actualContact.LastUpdateByUser, lastUser)
                .Set(actualContact => actualContact.LastUpdateAt, DateTime.UtcNow);

            return await _collection.UpdateOneAsync(filter, update);
        }


        public async Task<bool> CheckIfNIEUnique(string NIE, Guid? contactId)
        {
            var filter = Builders<Contact>.Filter.Eq("NIE", NIE);

            if (contactId.HasValue)
            {
                var idFilter = Builders<Contact>.Filter.Ne("Id", contactId);
                filter = filter & idFilter;
            }

            var count = await _collection.CountDocumentsAsync(filter);

            return count == 0;
        }

    }
}
