namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine
{
    using System;
    using Documents.Structs;

    public interface IMigration
    {
        DocumentVersion Version { get; }
        Type Type { get; }
    }
}