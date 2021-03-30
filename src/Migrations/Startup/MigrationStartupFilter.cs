namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Startup
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class MigrationStartupFilter : IStartupFilter
    {
        private readonly ILogger<MigrationStartupFilter> _logger;
        private readonly IMongoMigration _migration;

        public MigrationStartupFilter(IServiceScopeFactory serviceScopeFactory)
            : this(serviceScopeFactory, NullLoggerFactory.Instance)
        {
        }

        public MigrationStartupFilter(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
        {
            _migration = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IMongoMigration>();
            _logger = loggerFactory.CreateLogger<MigrationStartupFilter>();
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            try
            {
                _logger.LogInformation("Running migration. Please wait....");
                _migration.Run();
                _logger.LogInformation("Migration has been done");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Type {ExceptionType}", ex.GetType().ToString());
            }

            return next;
        }
    }
}