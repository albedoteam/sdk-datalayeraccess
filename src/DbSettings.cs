namespace AlbedoTeam.Sdk.DataLayerAccess
{
    using Abstractions;

    internal class DbSettings : IDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public bool RunMigrations { get; set; }
        public string DatabaseMigrationVersion { get; set; } = "0.0.0";
    }
}