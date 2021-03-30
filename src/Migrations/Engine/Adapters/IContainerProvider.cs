namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Adapters
{
    using System;

    internal interface IContainerProvider
    {
        object GetInstance(Type type);
    }
}