namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine.Database
{
    using System;
    using Documents.Structs;
    using MongoDB.Driver;

    public abstract class DatabaseMigration : IDatabaseMigration
    {
        protected DatabaseMigration(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }
        public Type Type => typeof(DatabaseMigration);
        public abstract void Up(IMongoDatabase db);
        public abstract void Down(IMongoDatabase db);
    }
}