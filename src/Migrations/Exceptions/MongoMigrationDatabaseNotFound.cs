namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;

    internal class MongoMigrationDatabaseNotFound
        : Exception
    {
        public MongoMigrationDatabaseNotFound(string databaseName, string valueConnectionString)
            : base($"Database could not be found for: '{databaseName}', database: '{valueConnectionString}'")
        {
        }
    }
}