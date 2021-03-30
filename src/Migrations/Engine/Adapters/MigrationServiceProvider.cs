namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Adapters
{
    using System;

    internal class MigrationServiceProvider : IContainerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetInstance(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}