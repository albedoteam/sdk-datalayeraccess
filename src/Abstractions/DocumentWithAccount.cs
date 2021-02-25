using MongoDB.Bson.Serialization.Attributes;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    [BsonIgnoreExtraElements]
    public abstract class DocumentWithAccount : Document, IDocumentWithAccount
    {
        public string AccountId { get; set; }
    }
}