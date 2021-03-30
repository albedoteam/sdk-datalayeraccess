namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    using MongoDB.Bson;

    public class FakeDocumentProjection
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}