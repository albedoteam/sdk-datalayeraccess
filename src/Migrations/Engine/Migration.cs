namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Engine
{
    using System;
    using Abstractions;
    using Document;

    [Obsolete]
    public abstract class Migration<TClass> : DocumentMigration<TClass> where TClass : class, IDocument
    {
        protected Migration(string version) : base(version)
        {
        }
    }
}