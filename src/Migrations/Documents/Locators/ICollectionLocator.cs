namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Locators
{
    using System;
    using System.Collections.Generic;
    using Structs;

    internal interface ICollectionLocator : ILocator<CollectionLocationInformation, Type>
    {
        IDictionary<Type, CollectionLocationInformation> GetLocatesOrEmpty();
    }
}