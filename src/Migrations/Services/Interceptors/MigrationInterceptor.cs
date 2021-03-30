namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services.Interceptors
{
    using Abstractions;
    using Engine.Document;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using Startup;

    internal class MigrationInterceptor<TDocument> : BsonClassMapSerializer<TDocument>
        where TDocument : class, IDocument
    {
        private readonly IDocumentVersionService _documentVersionService;
        private readonly IDocumentMigrationRunner _migrationRunner;
        private readonly IMigrationSettings _settings;

        public MigrationInterceptor(
            IDocumentMigrationRunner migrationRunner,
            IDocumentVersionService documentVersionService,
            IMigrationSettings settings)
            : base(BsonClassMap.LookupClassMap(typeof(TDocument)))
        {
            _migrationRunner = migrationRunner;
            _documentVersionService = documentVersionService;
            _settings = settings;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDocument value)
        {
            _documentVersionService.DetermineVersion(value);

            base.Serialize(context, args, value);
        }

        public override TDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // Performance? LatestVersion, dont do anything
            var document = BsonDocumentSerializer.Instance.Deserialize(context);

            if (_settings.RunMigrations)
                _migrationRunner.Run(typeof(TDocument), document);

            var migratedContext =
                BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));

            return base.Deserialize(migratedContext, args);
        }
    }
}