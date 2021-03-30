namespace AlbedoTeam.Sdk.DataLayerAccess
{
    using System;
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Migrations.Documents.Structs;
    using Migrations.Startup;
    using MongoDB.Driver;
    using Utils;

    public static class Setup
    {
        public static IServiceCollection AddDataLayerAccess(
            this IServiceCollection services,
            Action<IDbSettings> configureDb)
        {
            IDbSettings dbSettings = new DbSettings();
            configureDb.Invoke(dbSettings);

            services.AddSingleton(dbSettings);
            services.AddScoped(typeof(IDbContext<>), typeof(DbContext<>));

            services.AddScoped(typeof(IHelpers<>), typeof(Helpers<>));
            services.AddScoped(typeof(IHelpersWithAccount<>), typeof(HelpersWithAccount<>));
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepositoryImpl<>));

            // remove mongo client here?
            var mongoClient = new MongoClient(dbSettings.ConnectionString);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddMigration(new MigrationSettings
            {
                ConnectionString = dbSettings.ConnectionString,
                Database = dbSettings.DatabaseName,
                RunMigrations = dbSettings.RunMigrations,
                DatabaseMigrationVersion = new DocumentVersion(dbSettings.DatabaseMigrationVersion)
            });

            return services;
        }
    }
}