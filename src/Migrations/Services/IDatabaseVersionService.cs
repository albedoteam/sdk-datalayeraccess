namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services
{
    using Documents.Structs;
    using Engine.Database;
    using MongoDB.Driver;

    internal interface IDatabaseVersionService
    {
        DocumentVersion GetCurrentOrLatestMigrationVersion();
        DocumentVersion GetLatestDatabaseVersion(IMongoDatabase db);
        void Save(IMongoDatabase db, IDatabaseMigration migration);
        void Remove(IMongoDatabase db, IDatabaseMigration migration);
    }
}