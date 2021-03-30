namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Document
{
    using MongoDB.Bson;

    public interface IDocumentMigration : IMigration
    {
        void Up(BsonDocument document);
        void Down(BsonDocument document);
    }
}