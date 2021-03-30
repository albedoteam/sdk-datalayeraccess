namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services.Interceptors
{
    using System;
    using MongoDB.Bson.Serialization;

    internal interface IMigrationInterceptorFactory
    {
        IBsonSerializer Create(Type type);
    }
}