namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Exceptions
{
    using System;

    internal class InvalidVersionValueException : Exception
    {
        public InvalidVersionValueException(string value) :
            base($"Invalid value: {value}")
        {
        }
    }
}