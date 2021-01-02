﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }

        DateTime CreatedAt { get; }

        DateTime UpdatedAt { get; }

        bool IsDeleted { get; }

        DateTime DeletedAt { get; }
    }
}