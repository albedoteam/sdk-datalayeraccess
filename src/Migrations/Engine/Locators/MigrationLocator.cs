namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Locators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Documents.Structs;

    public abstract class MigrationLocator<TMigrationType> : IMigrationLocator<TMigrationType>
        where TMigrationType : class, IMigration
    {
        private IEnumerable<Assembly> _assemblies;
        private IDictionary<Type, IReadOnlyCollection<TMigrationType>> _migrations;
        protected IEnumerable<Assembly> Assemblies => _assemblies ??= GetAssemblies();

        protected virtual IDictionary<Type, IReadOnlyCollection<TMigrationType>> Migrations
        {
            get
            {
                if (_migrations == null)
                    Locate();

                // if (_migrations.NullOrEmpty())
                //     throw new NoMigrationsFoundException();

                return _migrations;
            }
            set => _migrations = value;
        }

        public IEnumerable<TMigrationType> GetMigrations(Type type)
        {
            Migrations.TryGetValue(type, out var migrations);
            return migrations ?? Enumerable.Empty<TMigrationType>();
        }

        public IEnumerable<TMigrationType> GetMigrationsGt(Type type, DocumentVersion version)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version > version)
                    .ToList();
        }

        public IEnumerable<TMigrationType> GetMigrationsGtEq(Type type, DocumentVersion version)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version >= version)
                    .ToList();
        }

        public IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version,
            DocumentVersion otherVersion)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version > version)
                    .Where(m => m.Version <= otherVersion)
                    .ToList();
        }

        public DocumentVersion GetLatestVersion(Type type)
        {
            var migrations = GetMigrations(type);

            var migrationTypes = migrations.ToList();
            return !migrationTypes.Any()
                ? DocumentVersion.Default()
                : migrationTypes.Max(m => m.Version);
        }

        public abstract void Locate();

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.GetDirectoryName(location);

            if (string.IsNullOrWhiteSpace(path))
                throw new DirectoryNotFoundException("Application directory could not be found");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var migrationAssemblies = Directory
                .GetFiles(path, "*.Migrations*.dll")
                .Select(Assembly.LoadFile);

            assemblies.AddRange(migrationAssemblies);

            return assemblies;
        }
    }
}