namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Structs
{
    public struct CollectionLocationInformation
    {
        public CollectionLocationInformation(string collection)
        {
            Collection = collection;
        }

        public string Collection { get; }
    }
}