namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Services.Interceptors
{
    using MongoDB.Bson.Serialization;

    internal interface IMigrationInterceptorProvider : IBsonSerializationProvider
    {
    }
}