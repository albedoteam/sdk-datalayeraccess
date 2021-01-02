using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IDbContext<TDocument> where TDocument : IDocument
    {
        IMongoCollection<TDocument> GetCollection();
    }
}