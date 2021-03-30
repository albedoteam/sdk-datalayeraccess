namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using System;
    using Migrations.Documents.Structs;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DocumentVersion Version { get; set; }

        DateTime CreatedAt { get; }

        DateTime? UpdatedAt { get; }

        bool IsDeleted { get; }

        DateTime? DeletedAt { get; }
    }
}