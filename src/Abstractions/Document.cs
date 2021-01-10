using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    [BsonIgnoreExtraElements]
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        // public DateTime CreatedAt => Id.CreationTime;

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Document()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}