namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Locators
{
    using System;
    using System.Collections.Generic;
    using Adapters;
    using Database;

    internal class DatabaseTypeMigrationDependencyLocator : TypeMigrationDependencyLocator<IDatabaseMigration>,
        IDatabaseTypeMigrationDependencyLocator
    {
        private IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> _migrations;

        public DatabaseTypeMigrationDependencyLocator(IContainerProvider containerProvider) : base(containerProvider)
        {
        }

        protected override IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> Migrations
        {
            get
            {
                if (_migrations == null) Locate();

                return _migrations;
            }
            set => _migrations = value;
        }
    }
}