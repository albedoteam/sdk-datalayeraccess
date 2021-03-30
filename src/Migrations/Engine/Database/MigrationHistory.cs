namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Database
{
    using Documents.Structs;
    using MongoDB.Bson;

    internal class MigrationHistory
    {
        public ObjectId Id { get; set; }
        public string MigrationId { get; set; }
        public DocumentVersion Version { get; set; }
    }
}