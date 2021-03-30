namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;

    internal class AlreadyInitializedException : Exception
    {
        public AlreadyInitializedException()
            : base("Migration was already initialized")
        {
        }
    }
}