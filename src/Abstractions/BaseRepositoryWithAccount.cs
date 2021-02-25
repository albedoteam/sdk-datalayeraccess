using System.Threading.Tasks;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public class BaseRepositoryWithAccount<TDocument> : BaseRepository<TDocument>, IBaseRepositoryWithAccount<TDocument>
        where TDocument : IDocumentWithAccount
    {
        public BaseRepositoryWithAccount(IDbContext<TDocument> context) : base(context)
        {
        }

        public async Task<TDocument> FindById(string accountId, string id, bool showDeleted = false)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);
            return await FindById(id, showDeleted, accountFilter);
        }
    }
}