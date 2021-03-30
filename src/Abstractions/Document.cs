namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using System;
    using Migrations.Documents.Structs;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class Document : IDocument
    {
        public Document()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public ObjectId Id { get; set; }
        public DocumentVersion Version { get; set; }

        // public DateTime CreatedAt => Id.CreationTime;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}