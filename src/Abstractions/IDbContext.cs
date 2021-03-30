namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using MongoDB.Driver;

    public interface IDbContext<TDocument> where TDocument : IDocument
    {
        IMongoCollection<TDocument> GetCollection();
    }
}