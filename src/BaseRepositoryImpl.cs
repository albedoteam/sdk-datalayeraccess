namespace AlbedoTeam.Sdk.DataLayerAccess
{
    using Abstractions;

    /// <summary>
    ///     This concret class is needed to use the IBaseRespositoryWithAccount
    /// </summary>
    /// <typeparam name="TDocument">Model</typeparam>
    public class BaseRepositoryImpl<TDocument> : BaseRepository<TDocument>
        where TDocument : class, IDocument, new()
    {
        public BaseRepositoryImpl(IDbContext<TDocument> context, IHelpers<TDocument> helpers) : base(context, helpers)
        {
        }
    }
}