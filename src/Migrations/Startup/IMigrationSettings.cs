namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Startup
{
    using Documents.Structs;
    using MongoDB.Driver;

    internal interface IMigrationSettings
    {
        string ConnectionString { get; set; }
        string Database { get; set; }
        DocumentVersion DatabaseMigrationVersion { get; set; }
        string VersionFieldName { get; set; }
        MongoClientSettings ClientSettings { get; set; }
        bool RunMigrations { get; set; }
    }
}