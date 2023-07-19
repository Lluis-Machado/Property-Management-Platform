using CompanyAPI.Contexts;
using CompanyAPI.Models;
using MongoDB.Driver;

namespace CompanyAPI.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IMongoCollection<Company> _collection;
        public CompanyRepository(MongoContext context)
        {
            var database = context.GetDataBase("companies");
            _collection = database.GetCollection<Company>("companies");
        }
        
        public async Task<Company> InsertOneAsync(Company company)
        {
            await _collection.InsertOneAsync(company);
            return company;
        }

        public async Task<List<Company>> GetAsync(bool includeDeleted = false)
        {
            FilterDefinition<Company> filter;

            if (includeDeleted)
            {
                filter = Builders<Company>.Filter.Empty;
            }
            else
            {
                filter = Builders<Company>.Filter.Eq(x => x.Deleted, false);
            }

            var result = await _collection.Find(filter).ToListAsync();
            return result;
        }


        public async Task<Company> GetByIdAsync(Guid id)
        {
            var filter = Builders<Company>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfAnyExistAsync(Guid id)
        {
            var filter = Builders<Company>.Filter.Eq(c => c.Id, id);

            return await _collection.Find(filter)
                .AnyAsync();
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            var filter = Builders<Company>.Filter.Eq(actualCompany => actualCompany.Id, company.Id);

            await _collection.ReplaceOneAsync(filter, company);

            return company;
        }

        public async Task<UpdateResult> SetDeleteAsync(Guid companyId, bool deleted, string lastUser)
        {
            var filter = Builders<Company>.Filter
                .Eq(actualCompany => actualCompany.Id, companyId);

            var update = Builders<Company>.Update
                .Set(actualCompany => actualCompany.Deleted, deleted)
                .Set(actualCompany => actualCompany.LastUpdateByUser, lastUser)
                .Set(actualCompany => actualCompany.LastUpdateAt, DateTime.UtcNow);

            return await _collection.UpdateOneAsync(filter, update);
        }
    }
}
