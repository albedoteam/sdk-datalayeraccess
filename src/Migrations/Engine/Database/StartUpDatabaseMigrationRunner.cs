namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Database
{
    using Exceptions;
    using MongoDB.Driver;
    using Startup;

    internal class StartUpDatabaseMigrationRunner : IStartUpDatabaseMigrationRunner
    {
        private readonly IMongoClient _client;
        private readonly string _databaseName;
        private readonly IDatabaseMigrationRunner _migrationRunner;

        public StartUpDatabaseMigrationRunner(
            IMigrationSettings settings,
            IDatabaseMigrationRunner migrationRunner)
            : this(migrationRunner)
        {
            if (settings.ConnectionString == null && settings.Database == null || settings.ClientSettings == null)
                throw new MongoMigrationNoMongoClientException();

            _client = settings.ClientSettings != null
                ? new MongoClient(settings.ClientSettings)
                : new MongoClient(settings.ConnectionString);

            _databaseName = settings.Database;
        }

        public StartUpDatabaseMigrationRunner(
            IMongoClient client,
            IMigrationSettings settings,
            IDatabaseMigrationRunner migrationRunner)
            : this(migrationRunner)
        {
            _client = client;
            if (settings.ConnectionString == null && settings.Database == null) return;

            _client = new MongoClient(settings.ConnectionString);
            _databaseName = settings.Database;
        }

        private StartUpDatabaseMigrationRunner(
            IDatabaseMigrationRunner migrationRunner)
        {
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            var databaseName = GetDatabase();
            _migrationRunner.Run(_client.GetDatabase(databaseName));
        }

        private string GetDatabase()
        {
            if (string.IsNullOrEmpty(_databaseName))
                throw new NoDatabaseNameFoundException();

            return _databaseName;
        }
    }
}