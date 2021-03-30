namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Startup
{
    using Documents.Locators;
    using Documents.Serializers;
    using Engine.Adapters;
    using Engine.Database;
    using Engine.Document;
    using Engine.Locators;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Services.Interceptors;

    internal static class MigrationExtensions
    {
        public static void AddMigration(this IServiceCollection services, IMigrationSettings settings = null)
        {
            RegisterDefaults(services, settings ?? new MigrationSettings());
        }

        private static void RegisterDefaults(IServiceCollection services, IMigrationSettings settings)
        {
            services.AddSingleton(settings);

            services.AddSingleton<IContainerProvider, MigrationServiceProvider>();
            services.AddSingleton(typeof(IMigrationLocator<>), typeof(TypeMigrationDependencyLocator<>));
            services.AddSingleton<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>();
            services.AddSingleton<ICollectionLocator, CollectionLocator>();
            services.AddSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>();
            services.AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>();

            services.AddTransient<IDatabaseVersionService, DatabaseVersionService>();
            services.AddTransient<IDocumentVersionService, DocumentVersionService>();
            services.AddTransient<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            services.AddTransient<DocumentVersionSerializer, DocumentVersionSerializer>();

            services.AddTransient<IStartUpDocumentMigrationRunner, StartUpDocumentMigrationRunner>();
            services.AddTransient<IDocumentMigrationRunner, DocumentMigrationRunner>();

            services.AddTransient<IStartUpDatabaseMigrationRunner, StartUpDatabaseMigrationRunner>();
            services.AddTransient<IDatabaseMigrationRunner, DatabaseMigrationRunner>();

            services.AddTransient<IMigrationInterceptorProvider, MigrationInterceptorProvider>();

            services.AddTransient<IMongoMigration, MongoMigration>();
            // services.AddTransient<IStartupFilter, MigrationStartupFilter>();

            services.AddScoped<IMigrationService, MigrationService>();

            // meow!!
            services.AddTransient<MongoMigrationStartupRunner>();

            var provider = services.BuildServiceProvider();
            var startup = provider.GetService<MongoMigrationStartupRunner>();
            startup.Run();
        }
    }
}