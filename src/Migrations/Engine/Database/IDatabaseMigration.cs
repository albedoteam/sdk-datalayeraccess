namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Database
{
    using MongoDB.Driver;

    public interface IDatabaseMigration : IMigration
    {
        void Up(IMongoDatabase db);
        void Down(IMongoDatabase db);
    }
}