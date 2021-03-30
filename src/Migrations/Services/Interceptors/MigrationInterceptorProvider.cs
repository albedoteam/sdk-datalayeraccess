namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services.Interceptors
{
    using System;
    using System.Collections;
    using Abstractions;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;

    internal class MigrationInterceptorProvider : IMigrationInterceptorProvider
    {
        private readonly IMigrationInterceptorFactory _migrationInterceptorFactory;

        public MigrationInterceptorProvider(IMigrationInterceptorFactory migrationInterceptorFactory)
        {
            _migrationInterceptorFactory = migrationInterceptorFactory;
        }

        public IBsonSerializer GetSerializer(Type type)
        {
            return ShouldBeMigrated(type)
                ? _migrationInterceptorFactory.Create(type)
                : null;
        }

        private static bool ShouldBeMigrated(Type type)
        {
            return ((IList) type.GetInterfaces()).Contains(typeof(IDocument)) && type != typeof(BsonDocument);
        }
    }
}