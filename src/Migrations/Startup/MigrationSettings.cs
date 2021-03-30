namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Startup
{
    using Documents.Structs;
    using MongoDB.Driver;

    internal class MigrationSettings : IMigrationSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public DocumentVersion DatabaseMigrationVersion { get; set; } = DocumentVersion.Empty();
        public string VersionFieldName { get; set; }
        public MongoClientSettings ClientSettings { get; set; }
        public bool RunMigrations { get; set; }
    }
}