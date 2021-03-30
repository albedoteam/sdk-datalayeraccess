namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Locators
{
    using System;
    using System.Collections.Generic;
    using Documents.Structs;

    public interface IMigrationLocator<TMigrationType> where TMigrationType : class, IMigration
    {
        IEnumerable<TMigrationType> GetMigrations(Type type);
        IEnumerable<TMigrationType> GetMigrationsGt(Type type, DocumentVersion version);
        IEnumerable<TMigrationType> GetMigrationsGtEq(Type type, DocumentVersion version);

        IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version,
            DocumentVersion otherVersion);

        DocumentVersion GetLatestVersion(Type type);
        void Locate();
    }
}