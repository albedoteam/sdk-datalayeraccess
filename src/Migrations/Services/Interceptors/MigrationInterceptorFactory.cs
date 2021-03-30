namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services.Interceptors
{
    using System;
    using Engine.Document;
    using MongoDB.Bson.Serialization;
    using Startup;

    internal class MigrationInterceptorFactory : IMigrationInterceptorFactory
    {
        private readonly IDocumentVersionService _documentVersionService;
        private readonly IDocumentMigrationRunner _migrationRunner;
        private readonly IMigrationSettings _settings;

        public MigrationInterceptorFactory(
            IDocumentMigrationRunner migrationRunner,
            IDocumentVersionService documentVersionService,
            IMigrationSettings settings)
        {
            _migrationRunner = migrationRunner;
            _documentVersionService = documentVersionService;
            _settings = settings;
        }

        public IBsonSerializer Create(Type type)
        {
            var genericType = typeof(MigrationInterceptor<>).MakeGenericType(type);

            var interceptor = Activator.CreateInstance(
                genericType,
                _migrationRunner,
                _documentVersionService,
                _settings);

            return interceptor as IBsonSerializer;
        }
    }
}