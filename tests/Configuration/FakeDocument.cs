namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    using Abstractions;
    using Attributes;

    [Collection("fakeDocs")]
    public class FakeDocument : Document
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
    }
}