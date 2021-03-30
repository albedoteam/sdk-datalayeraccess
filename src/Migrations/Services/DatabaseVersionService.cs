namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services
{
    using System.Linq;
    using Documents.Structs;
    using Engine.Database;
    using Engine.Locators;
    using MongoDB.Driver;
    using Startup;

    internal class DatabaseVersionService : IDatabaseVersionService
    {
        private const string MigrationsCollectionName = "_migrations";
        private readonly IDatabaseTypeMigrationDependencyLocator _migrationLocator;
        private readonly IMigrationSettings _migrationSettings;

        public DatabaseVersionService(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            IMigrationSettings migrationSettings)
        {
            _migrationLocator = migrationLocator;
            _migrationSettings = migrationSettings;
        }

        public DocumentVersion GetCurrentOrLatestMigrationVersion()
        {
            return _migrationSettings.DatabaseMigrationVersion > DocumentVersion.Empty()
                ? _migrationSettings.DatabaseMigrationVersion
                : _migrationLocator.GetLatestVersion(typeof(DatabaseMigration));
        }

        public DocumentVersion GetLatestDatabaseVersion(IMongoDatabase db)
        {
            var migrations = GetMigrationsCollection(db).Find(m => true).ToList();
            if (migrations == null || !migrations.Any()) return DocumentVersion.Default();

            return migrations.Max(m => m.Version);
        }

        public void Save(IMongoDatabase db, IDatabaseMigration migration)
        {
            GetMigrationsCollection(db).InsertOne(new MigrationHistory
            {
                MigrationId = migration.GetType().ToString(),
                Version = migration.Version
            });
        }

        public void Remove(IMongoDatabase db, IDatabaseMigration migration)
        {
            GetMigrationsCollection(db)
                .DeleteOne(Builders<MigrationHistory>.Filter.Eq(mh => mh.MigrationId, migration.GetType().ToString()));
        }

        private IMongoCollection<MigrationHistory> GetMigrationsCollection(IMongoDatabase db)
        {
            return db.GetCollection<MigrationHistory>(MigrationsCollectionName);
        }
    }
}