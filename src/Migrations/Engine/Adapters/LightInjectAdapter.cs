namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Adapters
{
    using System;
    using LightInject;

    internal class LightInjectAdapter : IContainerAdapter
    {
        private readonly IServiceContainer _container;

        public LightInjectAdapter(IServiceContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        public void Register<TInterface, TImplementation>() where TInterface : class
            where TImplementation : class, TInterface
        {
            _container.Register<TInterface, TImplementation>();
        }

        public void Register(Type serviceType, Type implementingType)
        {
            _container.Register(serviceType, implementingType);
        }

        public void RegisterInstance<TInterface>(object instance)
        {
            _container.RegisterInstance(typeof(TInterface), instance);
        }

        public void RegisterSingleton<TInterface, TImplementation>() where TInterface : class
            where TImplementation : class, TInterface
        {
            _container.Register<TInterface, TImplementation>(new PerContainerLifetime());
        }
    }
}