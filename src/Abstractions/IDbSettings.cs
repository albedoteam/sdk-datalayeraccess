namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IDbSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}