namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;

    internal class MongoMigrationNoMongoClientException : Exception
    {
        public MongoMigrationNoMongoClientException()
            : base("No MongoClient")
        {
        }
    }
}