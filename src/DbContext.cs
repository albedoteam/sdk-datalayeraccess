using System.Linq;
using System.Reflection;
using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using AlbedoTeam.Sdk.DataLayerAccess.Attributes;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public class DbContext<TDocument> : IDbContext<TDocument> where TDocument : IDocument
    {
        private readonly IMongoDatabase _db;

        public DbContext(IDbSettings settings)
        {
            _db = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<TDocument> GetCollection()
        {
            var collectionName = GetCollectionName(typeof(TDocument));
            return collectionName == null ? null : _db.GetCollection<TDocument>(collectionName);
        }

        private static string GetCollectionName(ICustomAttributeProvider documentType)
        {
            return ((BsonCollectionAttribute) documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute), true)
                .FirstOrDefault())?.CollectionName;
        }
    }
}