namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;

    internal class VersionStringToLongException : Exception
    {
        public VersionStringToLongException(string version) :
            base($"Versions must have format: major.minor.revision, this doesn't match: {version}")
        {
        }
    }
}