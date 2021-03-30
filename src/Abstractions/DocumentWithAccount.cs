namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class DocumentWithAccount : Document, IDocumentWithAccount
    {
        public string AccountId { get; set; }
    }
}