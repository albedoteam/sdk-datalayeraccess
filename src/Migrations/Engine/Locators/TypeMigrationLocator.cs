namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Locators
{
    using System;
    using System.Linq;
    using Document;
    using Extensions;

    internal class TypeMigrationLocator : MigrationLocator<IDocumentMigration>
    {
        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                    from type in assembly.GetTypes()
                    where typeof(IDocumentMigration).IsAssignableFrom(type) && !type.IsAbstract
                    select type).Distinct();

            Migrations = migrationTypes.Select(t => (IDocumentMigration)Activator.CreateInstance(t))
                .ToMigrationDictionary();
        }
    }
}