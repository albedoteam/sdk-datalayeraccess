namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Database
{
    using System;
    using System.Linq;
    using Documents.Structs;
    using Locators;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using MongoDB.Driver;
    using Services;

    internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
    {
        private readonly Type _databaseMigrationType = typeof(DatabaseMigration);
        private readonly IDatabaseVersionService _databaseVersionService;
        private readonly ILogger _logger;

        public DatabaseMigrationRunner(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            IDatabaseVersionService databaseVersionService)
            : this(migrationLocator, databaseVersionService, NullLoggerFactory.Instance)
        {
        }

        private DatabaseMigrationRunner(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            IDatabaseVersionService databaseVersionService,
            ILoggerFactory loggerFactory)
        {
            MigrationLocator = migrationLocator;
            _databaseVersionService = databaseVersionService;
            _logger = loggerFactory.CreateLogger<DatabaseMigrationRunner>();
        }

        private IDatabaseTypeMigrationDependencyLocator MigrationLocator { get; }

        public void Run(IMongoDatabase db)
        {
            _logger.LogInformation("Database migration started");
            var databaseVersion = _databaseVersionService.GetLatestDatabaseVersion(db);
            var currentOrLatest = _databaseVersionService.GetCurrentOrLatestMigrationVersion();

            if (databaseVersion == currentOrLatest) return;

            MigrateUpOrDown(db, databaseVersion, currentOrLatest);
            _logger.LogInformation("Database migration finished");
        }

        private void MigrateUpOrDown(
            IMongoDatabase db,
            DocumentVersion databaseVersion,
            DocumentVersion to)
        {
            if (databaseVersion > to)
            {
                MigrateDown(db, databaseVersion, to);
                return;
            }

            MigrateUp(db, databaseVersion, to);
        }

        private void MigrateUp(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
        {
            var migrations = MigrationLocator.GetMigrationsFromTo(_databaseMigrationType, currentVersion, toVersion)
                .ToList();

            foreach (var migration in migrations)
            {
                _logger.LogInformation("Database Migration Up: {CurrentVersion}:{MigrationVersion} ",
                    currentVersion.GetType().ToString(), migration.Version);

                migration.Up(db);
                _databaseVersionService.Save(db, migration);

                _logger.LogInformation("Database Migration Up finished successful: {Migration}:{MigrationVersion} ",
                    migration.GetType().ToString(), migration.Version);
            }
        }

        private void MigrateDown(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
        {
            var migrations = MigrationLocator
                .GetMigrationsGtEq(_databaseMigrationType, toVersion)
                .OrderByDescending(m => m.Version)
                .ToList();

            foreach (var migration in migrations)
            {
                _logger.LogInformation("Database Migration Down: {Migration}:{MigrationVersion} ",
                    migration.GetType().ToString(), migration.Version);

                migration.Down(db);
                _databaseVersionService.Remove(db, migration);

                _logger.LogInformation("Database Migration Down finished successful: {Migration}:{MigrationVersion} ",
                    migration.GetType().ToString(), migration.Version);
            }
        }
    }
}