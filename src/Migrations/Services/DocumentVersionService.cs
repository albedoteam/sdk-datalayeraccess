namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Documents.Locators;
    using Documents.Structs;
    using Engine.Document;
    using Engine.Locators;
    using Exceptions;
    using MongoDB.Bson;
    using Startup;

    internal class DocumentVersionService : IDocumentVersionService
    {
        private static readonly string VERSION_FIELD_NAME = "Version";

        private readonly IMigrationLocator<IDocumentMigration> _migrationLocator;

        private readonly IRuntimeVersionLocator _runtimeVersionLocator;

        private readonly IStartUpVersionLocator _startUpVersionLocator;

        private readonly string _versionFieldName;

        public DocumentVersionService(
            IMigrationLocator<IDocumentMigration> migrationLocator,
            IRuntimeVersionLocator runtimeVersionLocator,
            IStartUpVersionLocator startUpVersionLocator,
            IMigrationSettings migrationSettings)
        {
            _migrationLocator = migrationLocator;
            _runtimeVersionLocator = runtimeVersionLocator;
            _startUpVersionLocator = startUpVersionLocator;
            _versionFieldName = string.IsNullOrWhiteSpace(migrationSettings.VersionFieldName)
                ? VERSION_FIELD_NAME
                : migrationSettings.VersionFieldName;
        }

        public string GetVersionFieldName()
        {
            return _versionFieldName;
        }

        public DocumentVersion GetCurrentOrLatestMigrationVersion(Type type)
        {
            var latestVersion = _migrationLocator.GetLatestVersion(type);
            return GetCurrentVersion(type) ?? latestVersion;
        }

        public DocumentVersion GetCollectionVersion(Type type)
        {
            var version = GetCurrentOrLatestMigrationVersion(type);
            return _startUpVersionLocator.GetLocateOrNull(type) ?? version;
        }

        public DocumentVersion GetVersionOrDefault(BsonDocument document)
        {
            BsonValue value;
            document.TryGetValue(GetVersionFieldName(), out value);

            if (value != null && !value.IsBsonNull)
                return value.AsString;

            return DocumentVersion.Default();
        }

        public void SetVersion(BsonDocument document, DocumentVersion version)
        {
            document[GetVersionFieldName()] = version.ToString();
        }

        public void DetermineVersion<TClass>(TClass instance) where TClass : class, IDocument
        {
            var type = typeof(TClass);
            var documentVersion = instance.Version.ToString();
            var latestVersion = _migrationLocator.GetLatestVersion(type);
            var currentVersion = _runtimeVersionLocator.GetLocateOrNull(type) ?? latestVersion;

            if (documentVersion == currentVersion)
                return;

            if (documentVersion == latestVersion)
                return;

            if (DocumentVersion.Default() == documentVersion)
            {
                SetVersion(instance, currentVersion, latestVersion);
                return;
            }

            throw new VersionViolationException(currentVersion.ToString(), documentVersion, latestVersion);
        }

        public DocumentVersion DetermineLastVersion(DocumentVersion version, List<IDocumentMigration> migrations,
            int currentMigration)
        {
            if (migrations.Last() != migrations[currentMigration])
                return migrations[currentMigration + 1].Version;
            return version;
        }

        private DocumentVersion? GetCurrentVersion(Type type)
        {
            return _runtimeVersionLocator.GetLocateOrNull(type);
        }

        private static void SetVersion<TClass>(
            TClass instance,
            DocumentVersion? currentVersion,
            DocumentVersion latestVersion) where TClass : class, IDocument
        {
            if (currentVersion < latestVersion)
            {
                instance.Version = currentVersion.ToString();
                return;
            }

            instance.Version = latestVersion;
        }
    }
}