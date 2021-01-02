using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using AlbedoTeam.Sdk.DataLayerAccess.Attributes;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    [BsonCollection("fakeDocs")]
    public class FakeDocument : Document
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
    }
}