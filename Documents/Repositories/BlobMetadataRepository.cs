using DocumentsAPI.Contexts;
using DocumentsAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Document = DocumentsAPI.Models.Document;

namespace DocumentsAPI.Repositories
{
    public class BlobMetadataRepository : IBlobMetadataRepository
    {

        private readonly IMongoCollection<BlobMetadata> _collection;
        public BlobMetadataRepository(MongoContext context)
        {
            var database = context.GetDataBase("blobMetadata");
            _collection = database.GetCollection<BlobMetadata>("blobMetadata");
        }

        public async Task<BlobMetadata?> GetByBlobIdAsync(Guid blobId)
        {
            var filter = Builders<BlobMetadata>.Filter.Eq(b => b.BlobId, blobId);

            return await _collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BlobMetadata>> GetByContainerIdAsync(Guid containerId, Guid? folderId = null, bool includeDeleted = false)
        {
            var filter = Builders<BlobMetadata>.Filter.Eq(b => b.ContainerId, containerId);

            if (folderId != null) filter &= Builders<BlobMetadata>.Filter.Eq(b => b.FolderId, folderId);
            if (!includeDeleted) filter &= Builders<BlobMetadata>.Filter.Eq(b => b.Deleted, false);

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<BlobMetadata> InsertAsync(BlobMetadata blobMetadata, string username = "sa")
        {
            blobMetadata.CreatedByUser = username;
            blobMetadata.CreatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(blobMetadata);
            return blobMetadata;
        }

        public async Task<BlobMetadata> InsertAsync(Document document, Guid containerId, string username = "sa")
        {
            var blobMetadata = new BlobMetadata
            {
                BlobId = document.Id,
                FolderId = document.FolderId,
                ContainerId = containerId,
                DisplayName = document.Name ?? "NO NAME",
                CreatedByUser = document.CreatedByUser ?? username,
                CreatedAt = document.CreatedAt != DateTime.MinValue ? document.CreatedAt : DateTime.UtcNow
            };

            await _collection.InsertOneAsync(blobMetadata);
            return blobMetadata;
        }

        public async Task<UpdateResult> DeleteAsync(Guid blobId, string username = "sa")
        {
            var blob = await GetByBlobIdAsync(blobId);
            if (blob == null) throw new Exception($"Blob {blobId} not found");

            var update = Builders<BlobMetadata>.Update
                .Set(b => b.Deleted, true)
                .Set(b => b.LastUpdateByUser, username)
                .Set(b => b.LastUpdateAt, DateTime.UtcNow);

            var filter = Builders<BlobMetadata>.Filter.Eq(b => b.BlobId, blobId);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<UpdateResult> UndeleteAsync(Guid blobId, string username = "sa")
        {
            var update = Builders<BlobMetadata>.Update
                .Set(b => b.Deleted, false)
                .Set(b => b.LastUpdateByUser, username)
                .Set(b => b.LastUpdateAt, DateTime.UtcNow);

            var filter = Builders<BlobMetadata>.Filter.Eq(b => b.BlobId, blobId);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<BlobMetadata> UpdateAsync(BlobMetadata blobMetadata, string username = "sa")
        {
            var filter = Builders<BlobMetadata>.Filter.Eq(b => b.BlobId, blobMetadata.BlobId) & Builders<BlobMetadata>.Filter.Eq(b => b.ContainerId, blobMetadata.ContainerId);
            var blob = await _collection.FindAsync(filter);
            if (!blob.Any()) throw new Exception($"Blob {blobMetadata.BlobId} not found");

            var foundBlob = blob.First();

            blobMetadata.LastUpdateAt = DateTime.UtcNow;
            blobMetadata.LastUpdateByUser = username;

            if (blobMetadata.FolderId == Guid.Empty) blobMetadata.FolderId = foundBlob.FolderId;
            if (string.IsNullOrEmpty(blobMetadata.DisplayName)) blobMetadata.DisplayName = foundBlob.DisplayName;

            await _collection.ReplaceOneAsync(filter, blobMetadata);

            return blobMetadata;
        }

        public async Task<BlobMetadata> UpsertAsync(Guid blobId, Guid containerId, Guid? folderId, string displayName, string username = "sa")
        {
            var filter = Builders<BlobMetadata>.Filter.Eq(b => b.BlobId, blobId);

            var blobMetadata = new BlobMetadata { BlobId = blobId, ContainerId = containerId, FolderId = folderId, DisplayName = displayName };

            var foundBlobMetadata = await _collection.FindAsync(filter);
            if (foundBlobMetadata == null || !foundBlobMetadata.Any())
            {
                blobMetadata = await InsertAsync(blobMetadata);
            }
            else
            {
                blobMetadata = await UpdateAsync(blobMetadata);
            }

            return blobMetadata;

        }
        public async Task<IEnumerable<BlobMetadata>> SearchMetadata(string? displayName, Guid? folderId = null, Guid? containerId = null, bool includeDeleted = false)
        {

            //// Create index in MongoDB for each field if not already present

            //var currentIndexes = await _collection.Indexes.ListAsync();
            //if (currentIndexes.ToList()?.Count <= 1)
            //{
            //    //var indexes = new List<CreateIndexModel<BlobMetadata>>
            //    //{
            //    //    new CreateIndexModel<BlobMetadata>(Builders<BlobMetadata>.IndexKeys.Text("DisplayName")),
            //    //    new CreateIndexModel<BlobMetadata>(Builders<BlobMetadata>.IndexKeys.Text("FolderId")),
            //    //    new CreateIndexModel<BlobMetadata>(Builders<BlobMetadata>.IndexKeys.Text("ContainerId"))
            //    //};
            //    //await _collection.Indexes.CreateManyAsync(indexes);

            //    await _collection.Indexes.CreateOneAsync(new CreateIndexModel<BlobMetadata>(Builders<BlobMetadata>.IndexKeys.Text("DisplayName")));
            //}

            var filter = Builders<BlobMetadata>.Filter.Empty;

            //if (!string.IsNullOrEmpty(displayName)) filter &= Builders<BlobMetadata>.Filter.Text(displayName);
            if (!string.IsNullOrEmpty(displayName)) filter &= Builders<BlobMetadata>.Filter.Regex("DisplayName", new BsonRegularExpression(displayName, "i"));
            if (folderId != null) filter &= Builders<BlobMetadata>.Filter.Eq(b => b.FolderId, folderId);
            if (containerId != null) filter &= Builders<BlobMetadata>.Filter.Eq(b => b.ContainerId, containerId);
            if (!includeDeleted) filter &= Builders<BlobMetadata>.Filter.Eq(b => b.Deleted, false);

            try
            {
                var result = await _collection.FindAsync(filter);
                return result.ToList();

            }
            catch (MongoCommandException ex)
            {
                return new List<BlobMetadata>();
            }
        }
    }
}
