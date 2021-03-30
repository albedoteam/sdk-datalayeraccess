namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations
{
    using Documents.Locators;
    using Engine.Document;
    using Engine.Locators;
    using Services;

    internal class MongoMigration : IMongoMigration
    {
        private readonly ICollectionLocator _collectionLocator;
        private readonly IDatabaseTypeMigrationDependencyLocator _databaseMigrationLocator;
        private readonly IMigrationLocator<IDocumentMigration> _documentMigrationLocator;
        private readonly IMigrationService _migrationService;
        private readonly IRuntimeVersionLocator _runtimeVersionLocator;
        private readonly IStartUpVersionLocator _startUpVersionLocator;

        public MongoMigration(
            ICollectionLocator collectionLocator,
            IStartUpVersionLocator startUpVersionLocator,
            IMigrationLocator<IDocumentMigration> documentMigrationLocator,
            IDatabaseTypeMigrationDependencyLocator databaseMigrationLocator,
            IMigrationService migrationService,
            IRuntimeVersionLocator runtimeVersionLocator)
        {
            _collectionLocator = collectionLocator;
            _startUpVersionLocator = startUpVersionLocator;
            _documentMigrationLocator = documentMigrationLocator;
            _databaseMigrationLocator = databaseMigrationLocator;
            _migrationService = migrationService;
            _runtimeVersionLocator = runtimeVersionLocator;
        }

        public void Run()
        {
            _documentMigrationLocator.Locate();
            _databaseMigrationLocator.Locate();
            _runtimeVersionLocator.Locate();
            _collectionLocator.Locate();
            _startUpVersionLocator.Locate();

            _migrationService.Migrate();
        }
    }
}