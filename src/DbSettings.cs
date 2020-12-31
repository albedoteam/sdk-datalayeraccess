using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public class DbSettings : IDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}