namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Database
{
    using MongoDB.Driver;

    internal interface IDatabaseMigrationRunner
    {
        void Run(IMongoDatabase db);
    }
}