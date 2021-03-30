namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;
    using Documents.Structs;

    internal class VersionViolationException : Exception
    {
        public VersionViolationException(
            DocumentVersion currentVersion,
            DocumentVersion documentVersion,
            DocumentVersion latestVersion)
            : base($"Migration '{currentVersion}' contains duplicate version: {documentVersion}")
        {
        }
    }
}