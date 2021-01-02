using MongoDB.Bson;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    public class FakeDocumentProjection
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}