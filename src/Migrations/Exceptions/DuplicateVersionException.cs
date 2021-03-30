namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;

    internal class DuplicateVersionException : Exception
    {
        public DuplicateVersionException(string typeName, string version)
            : base($"Migration '{typeName}' contains duplicate version: {version}")
        {
        }
    }
}