namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Adapters
{
    using System;

    internal interface IContainerCollection
    {
        void Register<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface;
        void RegisterInstance<TInterface>(object instance);

        void RegisterSingleton<TInterface, TImplementation>() where TInterface : class
            where TImplementation : class, TInterface;

        void Register(Type serviceType, Type implementingType);
    }
}