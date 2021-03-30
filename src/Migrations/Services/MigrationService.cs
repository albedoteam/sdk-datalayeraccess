namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services
{
    using Documents.Serializers;
    using Engine.Database;
    using Engine.Document;
    using Interceptors;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Startup;

    internal class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly IMigrationInterceptorProvider _provider;
        private readonly DocumentVersionSerializer _serializer;
        private readonly IMigrationSettings _settings;
        private readonly IStartUpDatabaseMigrationRunner _startUpDatabaseMigrationRunner;
        private readonly IStartUpDocumentMigrationRunner _startUpDocumentMigrationRunner;

        public MigrationService(
            DocumentVersionSerializer serializer,
            IMigrationInterceptorProvider provider,
            IStartUpDocumentMigrationRunner startUpDocumentMigrationRunner,
            IStartUpDatabaseMigrationRunner startUpDatabaseMigrationRunner,
            IMigrationSettings settings)
            : this(serializer, provider, NullLoggerFactory.Instance)
        {
            _startUpDocumentMigrationRunner = startUpDocumentMigrationRunner;
            _startUpDatabaseMigrationRunner = startUpDatabaseMigrationRunner;
            _settings = settings;
        }

        private MigrationService(
            DocumentVersionSerializer serializer,
            IMigrationInterceptorProvider provider,
            ILoggerFactory loggerFactory)
        {
            _serializer = serializer;
            _provider = provider;
            _logger = loggerFactory.CreateLogger<MigrationService>();
        }

        public void Migrate()
        {
            BsonSerializer.RegisterSerializationProvider(_provider);
            RegisterSerializer();

            OnStartup();
        }

        private void OnStartup()
        {
            if (!_settings.RunMigrations)
                return;

            _startUpDatabaseMigrationRunner.RunAll();
            _startUpDocumentMigrationRunner.RunAll();
        }

        private void RegisterSerializer()
        {
            try
            {
                BsonSerializer.RegisterSerializer(_serializer.ValueType, _serializer);
            }
            catch (BsonSerializationException ex)
            {
                // Catch if Serializer was registered already ... not great, I know.
                // We have to do this, because there is always a default DocumentVersionSerialzer.
                // BsonSerializer.LookupSerializer(), does not work.

                _logger.LogError(ex, "Exception Type {ExceptionType}", ex.GetType().ToString());
            }
        }
    }
}