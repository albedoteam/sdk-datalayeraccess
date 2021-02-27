using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    /// <summary>
    /// This concret class is needed to use the IBaseRespositoryWithAccount
    /// </summary>
    /// <typeparam name="TDocument">Model</typeparam>
    public class BaseRepositoryImpl<TDocument> : BaseRepository<TDocument>
        where TDocument : IDocument
    {
        public BaseRepositoryImpl(IDbContext<TDocument> context, IHelpers<TDocument> helpers) : base(context, helpers)
        {
        }
    }
}