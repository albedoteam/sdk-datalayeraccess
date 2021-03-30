namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Document
{
    using System;
    using Documents.Structs;
    using MongoDB.Bson;

    internal interface IDocumentMigrationRunner
    {
        void Run(Type type, BsonDocument document, DocumentVersion to);
        void Run(Type type, BsonDocument document);
    }
}