namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Locators
{
    using System;
    using System.Linq;
    using Adapters;
    using Extensions;

    internal class TypeMigrationDependencyLocator<TMigrationType> : MigrationLocator<TMigrationType>
        where TMigrationType : class, IMigration
    {
        private readonly IContainerProvider _containerProvider;

        public TypeMigrationDependencyLocator(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                    from type in assembly.GetTypes()
                    where typeof(TMigrationType).IsAssignableFrom(type) && !type.IsAbstract
                    select type).Distinct(new TypeComparer());

            Migrations = migrationTypes.Select(GetMigrationInstance).ToMigrationDictionary();
        }

        private TMigrationType GetMigrationInstance(Type type)
        {
            var constructor = type.GetConstructors()[0];

            var args = constructor
                .GetParameters()
                .Select(o => o.ParameterType)
                .Select(o => _containerProvider.GetInstance(o))
                .ToArray();

            return Activator.CreateInstance(type, args) as TMigrationType;
        }
    }
}