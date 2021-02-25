using System.Threading.Tasks;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IBaseRepositoryWithAccount<TDocument> : IBaseRepository<TDocument>
        where TDocument : IDocumentWithAccount
    {
        Task<TDocument> FindById(string accountId, string id, bool showDeleted = false);
    }
}