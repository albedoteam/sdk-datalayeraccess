namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Locators
{
    internal interface ILocator<TReturnType, TTypeIdentifier>
        where TReturnType : struct
        where TTypeIdentifier : class
    {
        TReturnType? GetLocateOrNull(TTypeIdentifier identifier);
        void Locate();
    }
}