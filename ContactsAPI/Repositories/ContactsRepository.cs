using ContactsAPI.Contexts;
using ContactsAPI.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Reflection;

namespace ContactsAPI.Repositories
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly IMongoCollection<Contact> _collection;
        private readonly ILogger<ContactsRepository> _logger;
        public ContactsRepository(MongoContext context, ILogger<ContactsRepository> logger)
        {
            var database = context.GetDataBase("contacts");
            _collection = database.GetCollection<Contact>("contacts");
            _logger = logger;
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


        public async Task<Contact?> GetContactByIdAsync(Guid id)
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

        public async Task<IEnumerable<Contact>> SearchAsync(string query)
        {

            // TODO: Check whether or not to search in the BaseModel fields (created/updated user and date)

            // Split the query into individual search tokens
            string[] tokenize = query.Split(' ');

            // Tokens follow AND logic
            // Example: "Cristiano Ronaldo CR7" will look for a contact with "Cristiano" AND "Ronaldo" AND "CR7" in any of its fields


            // Check if index exists

            var indexes = await _collection.Indexes.ListAsync();
            if (indexes.ToList().Count == 1)
            {
                await _collection.Indexes.CreateOneAsync(new CreateIndexModel<Contact>(Builders<Contact>.IndexKeys.Text("$**")));
            }


            var foundContacts = new ConcurrentBag<Contact>();

            var propsNames = new List<string>
            {
                "FirstName",
                "LastName",
                "Email",
                "BirthDay"
            };


            var subProps1 = typeof(ContactAddress).GetProperties();
            var subProps2 = typeof(ContactBankInformation).GetProperties();
            var subProps3 = typeof(ContactIdentification).GetProperties();
            var subProps4 = typeof(ContactPhones).GetProperties();

            foreach (var addressProp in subProps1) propsNames.Add("Addresses." + addressProp.Name);
            foreach (var bankProp in subProps2) propsNames.Add("BankInformation." + bankProp.Name);
            foreach (var idProp in subProps3) propsNames.Add("Identifications." + idProp.Name);
            foreach (var phoneProp in subProps4) propsNames.Add("Phones." + phoneProp.Name);

            propsNames.Remove("Addresses");
            propsNames.Remove("BankInformation");
            propsNames.Remove("Identifications");
            propsNames.Remove("Phones");

            ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 8 };

            await Parallel.ForEachAsync(propsNames, parallelOptions: po, async (property, CancellationToken) =>
            //foreach (var property in propsNames)
            {
                for (int i = 0; i < tokenize.Length; i++)
                {
                    var search = await _collection.FindAsync(Builders<Contact>.Filter.Regex(property, new MongoDB.Bson.BsonRegularExpression(tokenize[i], "i")));
                    var searchResults = search.ToList();
                    searchResults.ForEach(elem => { if (!foundContacts.Contains(elem)) foundContacts.Add(elem); });

                }

                //foreach (string token in tokenize)
                //{
                //    var search = await _collection.FindAsync(Builders<Contact>.Filter.Regex(property, new MongoDB.Bson.BsonRegularExpression(query, "i")));

                //    await search.ForEachAsync(elem => { if (!foundContacts.Contains(elem)) foundContacts.Add(elem); });
                //}
            }
            );

            // Remove duplicates
            List<Contact> found = foundContacts.GroupBy(c => c.Id).Select(c => c.First()).ToList();

            _logger.LogInformation("TOTAL FOUND CONTACTS: " + foundContacts.Count);

            List<Contact> contactsToRemove = new();

            // If multiple tokens are present, do a second pass for filtering
            if (tokenize.Length > 1)
            {
                found.ToList().ForEach(contact =>
                {
                    bool keep = false;
                    for (int i = 1; i < tokenize.Length; i++) {
                        
                        foreach (PropertyInfo prop in contact.GetType().GetProperties())
                        {
                            var val = prop.GetValue(contact, null)?.ToString();
                            if (val != null && val.Contains(tokenize[i]))
                            {
                                keep = true;
                                break;
                            }
                        }

                        if (!keep)
                            contactsToRemove.Add(contact);
                    }

                });
            }

            return found.Except(contactsToRemove).ToList();
        }

    }
}
