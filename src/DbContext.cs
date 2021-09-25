namespace AlbedoTeam.Sdk.DataLayerAccess
{
    using System.Linq;
    using System.Reflection;
    using Abstractions;
    using Attributes;
    using MongoDB.Driver;

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
            return ((CollectionAttribute)documentType.GetCustomAttributes(
                    typeof(CollectionAttribute), true)
                .FirstOrDefault())?.Name;
        }
    }
}