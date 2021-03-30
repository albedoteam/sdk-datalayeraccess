namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations
{
    using System;
    using Microsoft.Extensions.Logging;
    using Startup;

    internal class MongoMigrationStartupRunner
    {
        private readonly ILogger<MongoMigrationStartupRunner> _logger;
        private readonly IMongoMigration _migration;
        private readonly IMigrationSettings _settings;

        public MongoMigrationStartupRunner(
            ILoggerFactory loggerFactory,
            IMongoMigration migration,
            IMigrationSettings settings)
        {
            _migration = migration;
            _settings = settings;
            _logger = loggerFactory.CreateLogger<MongoMigrationStartupRunner>();
        }

        public void Run()
        {
            try
            {
                _logger.LogInformation("Setting up migrations. Please wait...");

                if (_settings.RunMigrations)
                    _logger.LogInformation("Running migrations. This action may take a while to perform...");

                _migration.Run();

                if (_settings.RunMigrations)
                    _logger.LogInformation("Migrations have been made!");
                else
                    _logger.LogInformation("Migrations have been configured, but skipped! Please check your settings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Type {ExceptionType}", ex.GetType().ToString());
            }
        }
    }
}