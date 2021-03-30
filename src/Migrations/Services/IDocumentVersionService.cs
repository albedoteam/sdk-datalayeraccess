namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Documents.Structs;
    using Engine.Document;
    using MongoDB.Bson;

    internal interface IDocumentVersionService
    {
        string GetVersionFieldName();
        DocumentVersion GetCurrentOrLatestMigrationVersion(Type type);
        DocumentVersion GetCollectionVersion(Type type);
        DocumentVersion GetVersionOrDefault(BsonDocument document);
        void SetVersion(BsonDocument document, DocumentVersion version);

        void DetermineVersion<TClass>(TClass instance) where TClass : class, IDocument;

        DocumentVersion DetermineLastVersion(
            DocumentVersion version,
            List<IDocumentMigration> migrations,
            int currentMigration);
    }
}