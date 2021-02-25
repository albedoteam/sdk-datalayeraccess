namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IDocumentWithAccount : IDocument
    {
        string AccountId { get; set; }
    }
}